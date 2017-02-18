namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels.Pages
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Main page for the app. Shows a tree view for all the files in the Unity project as well as a 
    /// pie chart depicting how much space each file uses.
    /// </summary>
    internal class WorkspacePageViewModel : Screen
    {
        /// <summary>
        /// The is the line that appears before app size information in the Unity log file.
        /// </summary>
        private const string LogStartWord = "Used Assets and files from the Resources folder, sorted by uncompressed size:";

        private readonly IConductor conductor;

        /// <summary>
        /// Path to the editor log file.
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// App's original size. Used for comparison purposes in the UI.
        /// </summary>
        private float originalSize;
        
        private string originalSizeText;
        
        private string currentSizeText;
        
        private string savingsSizeText;

        public WorkspacePageViewModel(IConductor conductor, string fileName)
        {
            this.conductor = conductor;
            this.fileName = fileName;
            this.Roots = new BindableCollection<FileItemViewModel>();
        }

        /// <summary>
        /// List of root nodes for a tree view.
        /// </summary>
        public IObservableCollection<FileItemViewModel> Roots { get; private set; }

        /// <summary>
        /// Label showing the app's original size. This never changes.
        /// </summary>
        public string OriginalSizeText
        {
            get
            {
                return this.originalSizeText;
            }

            set
            {
                if (this.originalSizeText != value)
                {
                    this.originalSizeText = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        /// <summary>
        /// Label showing the app's current size after edits. This changes when the user toggles a checkbox.
        /// </summary>
        public string CurrentSizeText
        {
            get
            {
                return this.currentSizeText;
            }

            set
            {
                if (this.currentSizeText != value)
                {
                    this.currentSizeText = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        /// <summary>
        /// Label showing the current space savings. This changes when the user toggles a checkbox.
        /// </summary>
        public string SavingsSizeText
        {
            get
            {
                return this.savingsSizeText;
            }

            set
            {
                if (this.savingsSizeText != value)
                {
                    this.savingsSizeText = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        /// <summary>
        /// Hide elements less than the target size from the tree view and pie chart.
        /// </summary>
        public void FilterFilesLessThan(float size)
        {
            foreach (var root in this.Roots)
            {
                this.FilterInternal(root, size);
            }
        }

        /// <summary>
        /// Invoked when the page becomes active. Loads the log file and processes it.
        /// </summary>
        protected override async void OnActivate()
        {
            base.OnActivate();
            await this.LoadFile();
            this.UpdateSizeText(true);
            foreach (var root in this.Roots)
            {
                root.PropertyChanged += this.OnRootChanged;
            }
        }

        /// <summary>
        /// Convert Unity editor log file size line into metadata to process.
        /// </summary>
        /// <remarks>
        /// Data comes in like: " {size} {unit} {percent}% {path}/{file}"
        /// </remarks>
        private static bool GetPathAndMegabytesFrom(string line, out Tuple<string, float> data)
        {
            try
            {
                line = line.Trim();
                var size = line.Substring(0, line.IndexOf(' '));
                var unit = line.Substring(line.IndexOf(' ') + 1, 2);
                float megabytes = float.Parse(size);
                switch (unit)
                {
                    case "kb":
                        megabytes /= 1000;
                        break;
                }

                var path = line.Substring(line.IndexOf("% ") + 2);
                data = new Tuple<string, float>(path, megabytes);
                return true;
            }
            catch
            {
                data = new Tuple<string, float>(string.Empty, -1);
                return false;
            }
        }

        /// <summary>
        /// Open the log file and process it into a set of FileItemViewModels suitable for use
        /// in a tree view.
        /// </summary>
        private async Task LoadFile()
        {
            using (var stream = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                // Step 1: Read the raw input data and convert it into a list of <file names, file size> tuples.
                List<Tuple<string, float>> files = new List<Tuple<string, float>>();
                bool startProcessing = false;
                var line = string.Empty;
                while (!reader.EndOfStream)
                {
                    line = await reader.ReadLineAsync();

                    if (line.Trim() == WorkspacePageViewModel.LogStartWord)
                    {
                        // This is the line before the size breakdown
                        startProcessing = true;
                        continue;
                    }

                    if (!startProcessing)
                    {
                        // Still haven't reached the size breakdown
                        continue;
                    }

                    Tuple<string, float> data;
                    if (WorkspacePageViewModel.GetPathAndMegabytesFrom(line, out data))
                    {
                        files.Add(data);
                    }
                    else
                    {
                        // We've reached the end of the size breakdown
                        break;
                    }
                }

                // Step 2: Sort the list of files by file name.
                files.Sort(new PathAndSizeComparer());

                // Step 3: Contruct a tree based on files
                Dictionary<string, FileItemViewModel> cache = new Dictionary<string, FileItemViewModel>();
                List<FileItemViewModel> roots = new List<FileItemViewModel>();
                for (int i = 0; i < files.Count; i++)
                {
                    var filePath = files[i];

                    // Get the path components
                    var filePathParts = filePath.Item1.Split(Path.AltDirectorySeparatorChar);

                    // Create the leaf using the file name
                    var fileItemViewModel = new FileItemViewModel(
                        filePath.Item1,
                        filePathParts[filePathParts.Length - 1],
                        filePath.Item2);

                    // Go through each file and add every nodes in the tree for each folder as well as leaves for each file
                    var currentChild = fileItemViewModel;
                    for (int j = filePathParts.Length - 2; j >= 0; j--)
                    {
                        var ancestorName = filePathParts[j];
                        var ancestorId = currentChild.Id.Substring(0, currentChild.Id.LastIndexOf(Path.AltDirectorySeparatorChar));
                        if (!cache.ContainsKey(ancestorId))
                        {
                            cache[ancestorId] = new FileItemViewModel(ancestorId, ancestorName, 0);
                            if (j == 0)
                            {
                                roots.Add(cache[ancestorId]);
                            }
                        }

                        var ancestor = cache[ancestorId];
                        if (!ancestor.Children.Contains(currentChild))
                        {
                            currentChild.Parent = ancestor;
                            ancestor.Children.Add(currentChild);
                        }

                        currentChild = ancestor;
                    }
                }

                // Step 4: A little warm up to get the root properties in the proper state and ready to draw.
                foreach (var root in roots)
                {
                    root.IsEnabled = true;
                    root.Visibility = Visibility.Visible;
                    root.ChartVisibility = Visibility.Visible;
                }

                // Step 5: Add the root nodes to the observable list for the view to process
                this.Roots.AddRange(roots);
            }
        }

        private void OnRootChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FileItemViewModel.MegabytesPropertyName)
            {
                this.UpdateSizeText(false);
            }
        }

        private void UpdateSizeText(bool updateOriginal)
        {
            var size = 0f;
            var units = "mb";
            foreach (var root in this.Roots)
            {
                size += root.Megabytes;
            }

            if (size < 1)
            {
                size *= 1000;
                units = "kb";
            }

            if (updateOriginal)
            {
                this.originalSize = size;
                this.OriginalSizeText = string.Format("Original Size: {0:0.0} {1}", size, units);
            }

            this.CurrentSizeText = string.Format("Current Size: {0:0.0} {1}", size, units);

            size = this.originalSize - size;
            units = "mb";
            if (size < 1)
            {
                size *= 1000;
                units = "kb";
            }

            this.SavingsSizeText = string.Format("Total Reduction: {0:0.0} {1}", size, units);
        }

        private void FilterInternal(FileItemViewModel root, float minSize)
        {
            foreach (var child in root.Children)
            {
                this.FilterInternal(child, minSize);
            }

            if (minSize < 0)
            {
                root.Visibility = Visibility.Visible;
            }
            else if (root.Megabytes < minSize)
            {
                root.Visibility = Visibility.Collapsed;
            }
            else
            {
                root.Visibility = Visibility.Visible;
            }
        }
    }
}
