using NotificationsVisualizerLibrary.Renderers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NotificationsVisualizerLibrary.Controls
{
    internal partial class CallingButton : Button
    {
        private readonly Grid _grid;
        private readonly TextBlock _textBlock;
        private readonly Image _image;

        public CallingButton()
        {
            Width = 72;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Stretch;
            Height = 72;
            Background = new SolidColorBrush();

            _textBlock = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12
            };
            Grid.SetRow(_textBlock, 2);

            _image = new Image()
            {
                Stretch = Stretch.Uniform
            };

            _grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = new GridLength(24) },
                    new RowDefinition() { Height = new GridLength(3) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                },

                Children =
                {
                    _image,
                    _textBlock
                }
            };

            Content = _grid;
        }

        public string? Text
        {
            set => _textBlock.Text = value;
        }

        public string? ImageUri
        {
            set => _image.Source = ImageHelper.GetBitmap(value);
        }

        public bool IsAnswerButton
        {
            set
            {
                if (value)
                {
                    Background = new SolidColorBrush((Color)Application.Current.Resources["SystemColorHighlightColor"]);
                    Width = double.NaN;
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else
                {
                    Background = new SolidColorBrush();
                    Width = 72;
                    HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
        }
    }
}
