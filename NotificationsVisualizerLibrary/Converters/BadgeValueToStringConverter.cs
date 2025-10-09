using NotificationsVisualizerLibrary.Model;
using System;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class BadgeValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is BadgeValue ? (value as BadgeValue).ToString() : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is string ? BadgeValue.Parse(value as string) : BadgeValue.Parse(null);
        }
    }
}
