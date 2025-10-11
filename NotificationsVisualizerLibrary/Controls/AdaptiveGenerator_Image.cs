using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Renderers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace NotificationsVisualizerLibrary.Controls
{
    internal static class AdaptiveGenerator_Image
    {
        public static FrameworkElement GenerateImage(
            AdaptiveImage imageNode,
            double topMargin,
            bool isInsideGroup,
            out bool needsMargin)
        {
            needsMargin = !imageNode.HintRemoveMargin.GetValueOrDefault(false);

            FrameworkElement imageControl;
            BitmapImage? bmp = ImageHelper.GetBitmap(imageNode.Src);

            HintImageAlign hintAlign = imageNode.HintAlign;
            if (hintAlign == HintImageAlign.Default)
            {
                hintAlign = HintImageAlign.Stretch;
            }

            HintCrop crop = imageNode.HintCrop;
            if (crop == HintCrop.Circle)
            {
                CircleImageStretch stretch = hintAlign == HintImageAlign.Stretch
                    ? imageNode.SupportedFeatures.CircleImageVerticalStretch
                        ? CircleImageStretch.UniformToWidthOrHeight
                        : CircleImageStretch.UniformToWidth
                    : CircleImageStretch.None;
                imageControl = new CircleImage()
                {
                    Source = bmp,
                    Stretch = stretch
                };
            }
            else
            {
                imageControl = new AdaptiveImageControl()
                {
                    Source = bmp,
                    Stretch = hintAlign == HintImageAlign.Stretch ? (isInsideGroup ? AdaptiveImageStretch.UniformToWidth : AdaptiveImageStretch.Uniform) : AdaptiveImageStretch.None
                };
            }

            imageControl.HorizontalAlignment = hintAlign switch
            {
                HintImageAlign.Left => HorizontalAlignment.Left,
                HintImageAlign.Center => HorizontalAlignment.Center,
                HintImageAlign.Right => HorizontalAlignment.Right,
                _ => HorizontalAlignment.Stretch,//images fill available width
            };
            imageControl.VerticalAlignment = VerticalAlignment.Top; //images don't fill available height

            if (!imageNode.HintRemoveMargin.GetValueOrDefault(false))
            {
                imageControl.Margin = new Thickness(0, topMargin, 0, 0);
            }

            return imageControl;
        }
    }
}
