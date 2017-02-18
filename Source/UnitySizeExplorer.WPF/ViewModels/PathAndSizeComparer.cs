namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels
{
    using System;
    using System.Collections.Generic;

    internal class PathAndSizeComparer : IComparer<Tuple<string, float>>
    {
        public int Compare(Tuple<string, float> x, Tuple<string, float> y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }
}
