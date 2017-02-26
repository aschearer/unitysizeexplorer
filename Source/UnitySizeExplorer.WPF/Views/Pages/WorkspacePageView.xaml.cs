namespace SpottedZebra.UnitySizeExplorer.WPF.Views.Pages
{
    using LiveCharts;
    using LiveCharts.Configurations;
    using LiveCharts.Wpf;
    using Microsoft.Win32;
    using SpottedZebra.UnitySizeExplorer.WPF.ViewModels;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ViewModels.Pages;

    public partial class WorkspacePageView : UserControl
    {
        public static readonly RoutedUICommand Filter1Action = new RoutedUICommand("Filter1Action", "Filter1Action", typeof(WorkspacePageView));

        public static readonly RoutedUICommand Filter2Action = new RoutedUICommand("Filter2Action", "Filter2Action", typeof(WorkspacePageView));

        public static readonly RoutedUICommand Filter3Action = new RoutedUICommand("Filter3Action", "Filter3Action", typeof(WorkspacePageView));

        public static readonly RoutedUICommand ClearFilterAction = new RoutedUICommand("ClearFilterAction", "ClearFilterAction", typeof(WorkspacePageView));

        public static readonly RoutedUICommand ToggleSelectedAction = new RoutedUICommand("ToggleSelectedAction", "ToggleSelectedAction", typeof(WorkspacePageView));

        public WorkspacePageView()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.PieChart.Series = new SeriesCollection();
            var viewModel = this.DataContext as WorkspacePageViewModel;
            viewModel.Roots.CollectionChanged += this.OnCollectionChanged;
            bool shouldAddFocus = true;
            foreach (var root in viewModel.Roots)
            {
                this.AddFileToChart(root);
                if (shouldAddFocus)
                {
                    root.IsSelected = true;
                    this.TreeView.Focus();
                    shouldAddFocus = false;
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var workspace = this.DataContext as WorkspacePageViewModel;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    for (int i = 0; i < workspace.Roots.Count; i++)
                    {
                        var viewModel = workspace.Roots[i];
                        this.AddFileToChart(viewModel);
                        if (i == 0)
                        {
                            viewModel.IsSelected = true;
                            this.TreeView.Focus();
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void AddFileToChart(FileItemViewModel viewModel)
        {
            var mapper = new PieMapper<FileItemViewModel>().Value(f => f.Megabytes);
            var series = new PieSeries(mapper);
            series.Title = viewModel.Name;
            series.Visibility = viewModel.ChartVisibility;
            series.Values = new ChartValues<FileItemViewModel>() { viewModel };
            series.DataContext = viewModel;
            series.SetBinding(PieSeries.VisibilityProperty, FileItemViewModel.ChartVisibilityPropertyName);
            this.PieChart.Series.Add(series);

            foreach (var child in viewModel.Children)
            {
                this.AddFileToChart(child);
            }
        }
        
        private void OnFilter1(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = this.DataContext as WorkspacePageViewModel;
            viewModel.FilterFilesLessThan(1);
        }

        private void OnFilter2(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = this.DataContext as WorkspacePageViewModel;
            viewModel.FilterFilesLessThan(0.5f);
        }

        private void OnFilter3(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = this.DataContext as WorkspacePageViewModel;
            viewModel.FilterFilesLessThan(0.1f);
        }

        private void OnClearFilter(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = this.DataContext as WorkspacePageViewModel;
            viewModel.FilterFilesLessThan(-1);
        }

        private void OnToggleSelected(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = this.TreeView.SelectedItem as FileItemViewModel;
            selected.IsChecked = !selected.IsChecked;
        }

        private void OnClose(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnHideItem(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var fileItem = button.DataContext as FileItemViewModel;
            fileItem.Visibility = Visibility.Collapsed;
        }
    }
}
