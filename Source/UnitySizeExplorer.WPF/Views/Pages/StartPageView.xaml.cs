namespace SpottedZebra.UnitySizeExplorer.WPF.Views.Pages
{
    using Caliburn.Micro;
    using Microsoft.Win32;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ViewModels.Pages;

    public partial class StartPageView : UserControl
    {
        public static readonly RoutedUICommand OpenAction = new RoutedUICommand("OpenAction", "OpenAction", typeof(StartPageView));

        public StartPageView()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLOaded;
        }

        private void OnLOaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".log";
            openFileDialog.Filter = "Unity Editor.log file (*.log)|*.log";

            var result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var fileName = openFileDialog.FileName;
                ((StartPageViewModel)this.DataContext).OpenFile(fileName);
            }
        }

        private void OnFileDropped(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var fileName = files[0];
                
                ((StartPageViewModel)this.DataContext).OpenFile(fileName);
            }
        }

        private void OnClose(object sender, ExecutedRoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
