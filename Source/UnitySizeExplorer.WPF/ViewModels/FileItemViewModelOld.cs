/*
namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using LiveCharts;
    using LiveCharts.Wpf;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Views;

    internal class FileItemViewModelOld : INotifyPropertyChanged, IEquatable<FileItemViewModel>
    {
        public readonly string Id;

        public readonly PieSeries PieChartSeries;

        private bool isChecked;

        private bool isExpanded;

        private Visibility visibility;

        private readonly PieChart pieChart;

        private bool isEnabled;
        private bool isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileItemViewModelOld(
            string id, 
            string name, 
            float megabytes,
            PieChart pieChart)
            : this(id, name, megabytes, true, pieChart)
        {
        }

        public FileItemViewModelOld(
            string id, 
            string name, 
            float megabytes, 
            bool isChecked,
            PieChart pieChart)
        {
            this.Children = new List<FileItemViewModel>();
            this.Id = id;
            this.Name = name;
            this.Megabytes = megabytes;
            this.isChecked = isChecked;
            this.visibility = Visibility.Visible;
            this.pieChart = pieChart;

            var series = new PieSeries();
            series.Title = this.Name;
            series.Values = new ChartValues<float>() { this.Size };
            series.Visibility = Visibility.Collapsed;
            this.PieChartSeries = series;
            this.pieChart.Series.Add(this.PieChartSeries);
            this.IsEnabled = false;
        }

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
                    this.OnPropertyChanged("IsChecked");
                    this.OnPropertyChanged("Size");
                    this.OnPropertyChanged("NameAndSize");

                    // Refresh ancestors file size to account for our toggle
                    var ancestor = this.Parent;
                    while (ancestor != null)
                    {
                        ancestor.OnPropertyChanged("Size");
                        ancestor.OnPropertyChanged("NameAndSize");
                        ancestor = ancestor.Parent;
                    }

                    foreach (var child in this.Children)
                    {
                        child.IsEnabled = this.IsChecked;
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
                    this.OnPropertyChanged("IsExpanded");
                    if (this.IsExpanded)
                    {
                        this.PieChartSeries.Visibility = Visibility.Collapsed;
                        foreach (var child in this.Children)
                        {
                            if (this.IsEnabled &&
                                child.IsChecked && 
                                child.Visibility == Visibility.Visible && 
                                !child.IsExpanded)
                            {
                                child.PieChartSeries.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                    {
                        this.PieChartSeries.Visibility = this.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
                        foreach (var child in this.Children)
                        {
                            child.PieChartSeries.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

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
                }

                this.OnPropertyChanged("Visibility");
                this.OnPropertyChanged("Size");
                this.OnPropertyChanged("NameAndSize");
            }
        }

        public string Name { get; private set; }

        public float Size
        {
            get
            {
                float size = this.Megabytes;
                foreach (var child in this.Children)
                {
                    if (child.IsChecked && child.visibility == Visibility.Visible)
                    {
                        size += child.Size;
                    }
                }

                return size;
            }
        }

        public string NameAndSize
        {
            get
            {
                float size = this.Size;
                string units = "mb";
                if (size < 1)
                {
                    size *= 1000;
                    units = "kb";
                }

                return string.Format("{0} – {1:0.0} {2}", this.Name, size, units);
            }
        }

        public float Megabytes { get; private set; }

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
                    this.OnPropertyChanged("IsEnabled");
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
                    this.OnPropertyChanged("IsSelected");
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

        protected void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }

            switch (property)
            {
                case "Visibility":
                case "Size":
                case "IsChecked":
                case "IsEnabled":
                    this.PieChartSeries.Values = new ChartValues<float>() { this.Size };

                    if (this.Visibility == Visibility.Collapsed || !this.IsChecked || this.IsExpanded || !this.IsEnabled)
                    {
                        this.PieChartSeries.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        bool isAnyParentCollapsed = false;
                        var parent = this.Parent;
                        while (parent != null)
                        {
                            if (!parent.IsExpanded)
                            {
                                isAnyParentCollapsed = true;
                                break;
                            }

                            parent = parent.Parent;
                        }

                        if (!isAnyParentCollapsed)
                        {
                            this.PieChartSeries.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.PieChartSeries.Visibility = Visibility.Collapsed;
                        }
                    }

                    break;
            }
        }
    }
}

*/