namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using System.Collections.Generic;

    public class FileItemInfoNameComparer : IComparer<FileItemInfo>
    {
        public int Compare(FileItemInfo x, FileItemInfo y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}