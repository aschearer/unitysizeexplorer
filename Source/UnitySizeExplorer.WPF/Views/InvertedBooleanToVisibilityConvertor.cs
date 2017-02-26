namespace SpottedZebra.UnitySizeExplorer.WPF.Views
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class InvertedBooleanToVisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
