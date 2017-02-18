namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using System.Windows;
    using Caliburn.Micro;
    using Pages;

    internal class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            this.ActivateItem(new StartPageViewModel(this));
            this.DisplayName = "Unity Size Explorer";
        }
    }
}
