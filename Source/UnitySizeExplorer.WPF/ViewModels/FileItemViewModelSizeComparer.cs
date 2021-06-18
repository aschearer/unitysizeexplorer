namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using System.Collections.Generic;

    internal class FileItemViewModelSizeComparer : IComparer<FileItemViewModel>
    {
        public int Compare(FileItemViewModel x, FileItemViewModel y)
        {
            return y.Megabytes.CompareTo(x.Megabytes);
        }
    }
}
