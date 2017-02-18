namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using LiveCharts;

    internal class FileItemViewModel : PropertyChangedBase, 
                                       IEquatable<FileItemViewModel>,
                                       IObservableChartPoint
    {
        public const string MegabytesPropertyName = "Megabytes";

        public const string IsCheckedPropertyName = "IsChecked";

        public const string NameAndSizePropertyName = "NameAndSize";

        public const string VisibilityPropertyName = "Visibility";

        public const string ChartVisibilityPropertyName = "ChartVisibility";

        public readonly string Id;

        private bool isChecked;

        private bool isExpanded;

        private Visibility visibility;

        private Visibility chartVisibility;

        private bool isEnabled;

        private bool isSelected;

        public FileItemViewModel(
            string id, 
            string name, 
            float megabytes)
            : this(id, name, megabytes, true)
        {
        }

        public FileItemViewModel(
            string id, 
            string name, 
            float megabytes, 
            bool isChecked)
        {
            this.Children = new List<FileItemViewModel>();
            this.Id = id;
            this.Name = name;
            this.MegabytesSelf = megabytes;
            this.isChecked = isChecked;
            this.visibility = Visibility.Visible;
            this.chartVisibility = Visibility.Collapsed;
            this.IsEnabled = false;
        }

        public event System.Action PointChanged;

        public FileItemViewModel Parent { get; set; }

        public List<FileItemViewModel> Children { get; private set; }

        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.NotifyOfPropertyChange(FileItemViewModel.IsCheckedPropertyName);
                    this.NotifyOfPropertyChange(FileItemViewModel.MegabytesPropertyName);
                    this.NotifyOfPropertyChange(FileItemViewModel.NameAndSizePropertyName);
                    this.PointChanged?.Invoke();

                    if (!this.IsExpanded && this.Visibility == Visibility.Visible)
                    {
                        this.ChartVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                    }

                    // Refresh ancestors file size to account for our toggle
                    var ancestor = this.Parent;
                    while (ancestor != null)
                    {
                        ancestor.NotifyOfPropertyChange(FileItemViewModel.MegabytesPropertyName);
                        ancestor.PointChanged?.Invoke();

                        ancestor.NotifyOfPropertyChange(FileItemViewModel.NameAndSizePropertyName);
                        ancestor = ancestor.Parent;
                    }

                    foreach (var child in this.Children)
                    {
                        child.IsEnabled = this.IsChecked;
                        if (this.IsExpanded)
                        {
                            if (this.IsChecked)
                            {
                                child.ChartVisibility = child.IsEnabled &&
                                                        child.IsChecked &&
                                                        child.visibility == Visibility.Visible
                                                      ? Visibility.Visible
                                                      : Visibility.Collapsed;
                            }
                            else
                            {
                                child.ChartVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    this.NotifyOfPropertyChange();
                    if (this.IsExpanded)
                    {
                        this.ChartVisibility = Visibility.Collapsed;
                        foreach (var child in this.Children)
                        {
                            if (this.IsEnabled &&
                                this.IsChecked &&
                                child.IsChecked && 
                                child.Visibility == Visibility.Visible && 
                                !child.IsExpanded)
                            {
                                child.ChartVisibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                    {
                        this.ChartVisibility = this.IsEnabled && this.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                        foreach (var child in this.Children)
                        {
                            child.ChartVisibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Whether the node should be visible in the tree view.
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                return this.visibility;
            }

            set
            {
                if (this.visibility != value)
                {
                    this.visibility = value;
                    if (value == Visibility.Collapsed)
                    {
                        this.ChartVisibility = value;
                    }
                    else
                    {
                        this.ChartVisibility = this.IsChecked && !this.IsExpanded ? Visibility.Visible : Visibility.Collapsed;
                    }

                    this.NotifyOfPropertyChange(FileItemViewModel.VisibilityPropertyName);
                    this.NotifyOfPropertyChange(FileItemViewModel.MegabytesPropertyName);
                    this.NotifyOfPropertyChange(FileItemViewModel.NameAndSizePropertyName);
                    this.PointChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Whether the node should be visible in the chart.
        /// </summary>
        public Visibility ChartVisibility
        {
            get
            {
                return this.chartVisibility;
            }

            set
            {
                if (this.chartVisibility != value)
                {
                    this.chartVisibility = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public string Name { get; private set; }

        /// <summary>
        /// Returns this node's size. Zero unless a leaf node.
        /// </summary>
        public float MegabytesSelf { get; private set; }

        /// <summary>
        /// Returns the size of this node and all its children.
        /// </summary>
        public float Megabytes
        {
            get
            {
                float size = this.MegabytesSelf;
                foreach (var child in this.Children)
                {
                    if (child.IsChecked)
                    {
                        size += child.Megabytes;
                    }
                }

                return size;
            }
        }

        public string NameAndSize
        {
            get
            {
                float size = this.Megabytes;
                string units = "mb";
                if (size < 1)
                {
                    size *= 1000;
                    units = "kb";
                }

                return string.Format("{0} – {1:0.0} {2}", this.Name, size, units);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                    this.NotifyOfPropertyChange();
                    foreach (var child in this.Children)
                    {
                        child.IsEnabled = value;
                    }
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public bool Equals(FileItemViewModel other)
        {
            return other != null && other.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var fileItemViewModel = obj as FileItemViewModel;
            return fileItemViewModel != null && fileItemViewModel.Id.Equals(this.Id);
        }
    }
}
