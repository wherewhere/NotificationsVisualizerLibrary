using NotificationsVisualizerLibrary.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class BadgeValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            value is not BadgeValue badgeValue ? Visibility.Collapsed : badgeValue.HasBadge() ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
