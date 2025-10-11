using System;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class InvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            -(double)value;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }

    public sealed partial class FontSizeToHeightConverter : IValueConverter
    {
        private const double COEFF = 0.715;

        public object Convert(object value, Type targetType, object parameter, string language) =>
            (double)value * COEFF;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }

    public sealed partial class FontSizeToLineHeightConverter : IValueConverter
    {
        private const double COEFF = 0.875;

        public object Convert(object value, Type targetType, object parameter, string language) =>
            double.Parse(value.ToString()!) * COEFF;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
