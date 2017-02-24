namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels.Pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;

    using Caliburn.Micro;

    /// <summary>
    ///     Main page for the app. Shows a tree view for all the files in the Unity project as well as a
    ///     pie chart depicting how much space each file uses.
    /// </summary>
    internal class WorkspacePageViewModel : Screen
    {
        private readonly IConductor conductor;

        /// <summary>
        ///     Path to the editor log file.
        /// </summary>
        private readonly string fileName;

        private string currentSizeText;

        /// <summary>
        ///     App's original size. Used for comparison purposes in the UI.
        /// </summary>
        private float originalSize;

        private string originalSizeText;

        private string savingsSizeText;

        public WorkspacePageViewModel(IConductor conductor, string fileName)
        {
            this.conductor = conductor;
            this.fileName = fileName;
        }

        /// <summary>
        ///     List of root nodes for a tree view.
        /// </summary>
        public IObservableCollection<FileItemViewModel> Roots { get; private set; }

        /// <summary>
        ///     Label showing the app's original size. This never changes.
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
        ///     Label showing the app's current size after edits. This changes when the user toggles a checkbox.
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
        ///     Label showing the current space savings. This changes when the user toggles a checkbox.
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
        ///     Hide elements less than the target size from the tree view and pie chart.
        /// </summary>
        public void FilterFilesLessThan(float size)
        {
            foreach (var root in this.Roots)
            {
                this.FilterInternal(root, size);
            }
        }

        /// <summary>
        ///     Invoked when the page becomes active. Loads the log file and processes it.
        /// </summary>
        protected override async void OnActivate()
        {
            base.OnActivate();
            this.Roots = await this.LoadFile();
            this.UpdateSizeText(true);
            foreach (var root in this.Roots)
            {
                root.PropertyChanged += this.OnRootChanged;
            }
        }

        private static void PrepareRoots(List<FileItemViewModel> roots)
        {
            // Step 4: A little warm up to get the root properties in the proper state and ready to draw.
            foreach (var root in roots)
            {
                root.IsEnabled = true;
                root.Visibility = Visibility.Visible;
                root.ChartVisibility = Visibility.Visible;
            }
        }

        private static List<FileItemViewModel> BuildTreeFromFiles(List<Tuple<string, float>> files)
        {
            // Step 3: Contruct a tree based on files
            var cache = new Dictionary<string, FileItemViewModel>();
            var roots = new List<FileItemViewModel>();
            for (var i = 0; i < files.Count; i++)
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
                for (var j = filePathParts.Length - 2; j >= 0; j--)
                {
                    var ancestorName = filePathParts[j];
                    var ancestorId = currentChild.Id.Substring(
                        0,
                        currentChild.Id.LastIndexOf(Path.AltDirectorySeparatorChar));
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

            return roots;
        }

        /// <summary>
        ///     Open the log file and process it into a set of FileItemViewModels suitable for use
        ///     in a tree view.
        /// </summary>
        private async Task<BindableCollection<FileItemViewModel>> LoadFile()
        {
            using (var stream = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var files = await FileBuilder.BuildFilesFromStream(stream);

                var roots = BuildTreeFromFiles(files);

                PrepareRoots(roots);

                // Step 5: Add the root nodes to the observable list for the view to process
                var output = new BindableCollection<FileItemViewModel>();
                output.AddRange(roots);
                return output;
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