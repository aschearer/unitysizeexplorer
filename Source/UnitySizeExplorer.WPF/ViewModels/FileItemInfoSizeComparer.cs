namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using System.Collections.Generic;

    public class FileItemInfoSizeComparer : IComparer<FileItemInfo>
    {
        public int Compare(FileItemInfo x, FileItemInfo y)
        {
            return y.Megabytes.CompareTo(x.Megabytes);
        }
    }
}
