namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using LiveCharts;

    /// <summary>
    /// Represents an item in the tree view as well as a slice in the pie chart.
    /// </summary>
    /// <remarks>
    /// Items are visible in the tree view whenever their Visibility != Collapsed.
    /// Items are visible in the pie chart whenever:
    ///   1. They are visible
    ///   2. They are checked
    ///   3. They are not expanded
    ///   4. All of their ancestors are:
    ///     i. Visible
    ///     ii. Checked
    ///     iii. Expanded
    /// 
    /// Whenever a node changes:
    ///   * Visibility
    ///   * IsChecked
    ///   * IsExpanded
    ///  Then that node and all its descendents must recalculate ChartVisibility.
    ///  
    /// An Item's size is the sum of its size and the size of all its descendents.
    /// 
    /// Whenever a node changes:
    ///   * Visibility
    ///   * IsChecked
    ///   * IsExpanded
    /// 
    /// Then that node and all its ancestors must recalculate NameAndSize and Megabytes.
    /// </remarks>
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
                    this.RecalculateChartVisility();
                    this.RecalculateMegabytes();
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
                    this.RecalculateChartVisility();
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

                    this.NotifyOfPropertyChange();
                    this.PointChanged?.Invoke();
                    this.RecalculateChartVisility();
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

        /// <summary>
        /// Evaluate every ancestor and returns true if they are all:
        ///  * Visible
        ///  * Checked
        ///  * Expanded
        /// </summary>
        private bool AncestorsAreExpandedCheckedAndVisible
        {
            get
            {
                bool isExpanded = true;
                var parent = this.Parent;
                while (parent != null)
                {
                    if (!parent.isExpanded ||
                        !parent.IsChecked ||
                        parent.Visibility != Visibility.Visible)
                    {
                        isExpanded = false;
                        break;
                    }

                    parent = parent.Parent;
                }

                return isExpanded;
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

        private void RecalculateMegabytes()
        {
            // Update my size
            this.NotifyOfPropertyChange(FileItemViewModel.MegabytesPropertyName);
            this.NotifyOfPropertyChange(FileItemViewModel.NameAndSizePropertyName);
            this.PointChanged?.Invoke();

            // Update all my ancestors size
            if (this.Parent != null)
            {
                this.Parent.RecalculateMegabytes();
            }
        }

        private void RecalculateChartVisility()
        {
            // Update my chart visibility
            this.ChartVisibility = this.Visibility == Visibility.Visible &&
                                   this.IsChecked &&
                                   !this.IsExpanded &&
                                   this.AncestorsAreExpandedCheckedAndVisible
                                 ? Visibility.Visible
                                 : Visibility.Collapsed;

            // Update all my descedents chart visibility
            foreach (var child in this.Children)
            {
                child.RecalculateChartVisility();
            }
        }
    }
}
