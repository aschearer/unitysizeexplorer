namespace SpottedZebra.UnitySizeExplorer.WPF.Views.Pages
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microsoft.Win32;

    using ViewModels.Pages;

    public partial class StartPageView : UserControl
    {
        public static readonly RoutedUICommand OpenAction = new RoutedUICommand(
            "OpenAction",
            "OpenAction",
            typeof(StartPageView));

        public StartPageView()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLOaded;
        }

        private static string GetDefaultUnityLogPath()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var defaultPath = Path.Combine(appdata, "Unity", "Editor");
            return defaultPath;
        }

        private void OnLOaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = GetDefaultUnityLogPath();
            openFileDialog.DefaultExt = ".log";
            openFileDialog.Filter = "Unity Editor.log file (*.log)|*.log";

            var result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var fileName = openFileDialog.FileName;
                ((StartPageViewModel)this.DataContext).OpenFile(fileName);
            }
        }

        private void OnFileDropped(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var fileName = files[0];

                ((StartPageViewModel)this.DataContext).OpenFile(fileName);
            }
        }

        private void OnClose(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}