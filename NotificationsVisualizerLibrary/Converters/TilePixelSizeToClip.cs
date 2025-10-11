using System;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class TilePixelSizeToClip : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Size s)
            {
                Size clipSize = new(s.Width * 3, s.Height);

                return new RectangleGeometry()
                {
                    Rect = new Rect(new Point(s.Width * -1, 0), clipSize)
                };
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
