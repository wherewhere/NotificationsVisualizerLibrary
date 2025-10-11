using System;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class TileSizeToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            value is not string tileSize
                ? value
                : tileSize.ToLower() switch
                {
                    "small" => "TileSmall",
                    "medium" => "TileMedium",
                    "wide" => "TileWide",
                    "large" => "TileLarge (Desktop Only)",
                    _ => tileSize,
                };

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
