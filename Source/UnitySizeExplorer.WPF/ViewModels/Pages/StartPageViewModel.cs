namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels.Pages
{
    using System;
    using Caliburn.Micro;

    /// <summary>
    /// App's first page. Prompts the user to open a Unity Editor log file.
    /// </summary>
    internal class StartPageViewModel : Screen
    {
        private readonly IConductor conductor;

        public StartPageViewModel(IConductor conductor)
        {
            this.conductor = conductor;
        }

        /// <summary>
        /// Invoked when the user has provided a Unity Editor log file.
        /// </summary>
        /// <param name="fileName">Path to a Unity Editor log file.</param>
        internal void OpenFile(string fileName)
        {
            this.conductor.ActivateItem(new WorkspacePageViewModel(this.conductor, fileName));
        }
    }
}
