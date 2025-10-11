using NotificationsVisualizerLibrary.Model;
using System;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class BadgeValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            value is BadgeValue badgeValue ? badgeValue.ToString() : "";

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            value is string @string ? BadgeValue.Parse(@string) : BadgeValue.Parse(null);
    }
}
