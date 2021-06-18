namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using System.Collections.Generic;

    public struct FileItemInfo
    {
        public string Name;
        public float Megabytes;

        public FileItemInfo(string name, float megabytes)
        {
            this.Name = name;
            this.Megabytes = megabytes;
        }
    }
}
