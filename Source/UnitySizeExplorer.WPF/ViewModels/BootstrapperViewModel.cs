namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using System.Windows;
    using Caliburn.Micro;

    internal class BootstrapperViewModel : BootstrapperBase
    {
        public BootstrapperViewModel()
        {
            this.Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>();
        }
    }
}
