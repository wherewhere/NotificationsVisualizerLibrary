using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Helpers;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    internal sealed partial class PreviewTileNotificationRaw : UserControl
    {
        private const int BRANDING_HEIGHT = 28;

        public PreviewTileNotificationRaw() => InitializeComponent();

        private AdaptiveBinding _binding;
        private TileSize _tileSize;

        /// <summary>
        /// This should only be called once. For new tile notification content, create a new instance of this element.
        /// </summary>
        /// <param name="tileSize"></param>
        /// <param name="visualElements"></param>
        /// <param name="isBrandingVisible"></param>
        /// <param name="binding"></param>
        public void InitializeFromXml(TileSize tileSize, PreviewTileVisualElements visualElements, bool isBrandingVisible, AdaptiveBinding binding)
        {
            _binding = binding ?? throw new ArgumentNullException("binding");
            _tileSize = tileSize;

            // Set the background color
            TileContentContainer.Background = new SolidColorBrush(visualElements.BackgroundColor);

            UsingPeek = false;

            if (binding.Container != null)
            {
                AdaptiveContainer container = binding.Container;

                // Calculate the tile margin
                Thickness margin = new Thickness(GetExternalMargin());

                if (isBrandingVisible)
                {
                    switch (tileSize)
                    {
                        case TileSize.Small:
                            margin.Bottom = BRANDING_HEIGHT - 4;
                            break;

                        default:
                            margin.Bottom = BRANDING_HEIGHT;
                            break;
                    }
                }

                // Render the adaptive
                TileContent.Child = AdaptiveRenderer.Render(container, margin);

                // Background image
                IEnumerable<AdaptiveImage> allImages = container.GetAllDescendants().OfType<AdaptiveImage>();
                AdaptiveImage backgroundImage = allImages.FirstOrDefault(i => i.Placement == Placement.Background);
                AdaptiveImage peekImage = allImages.FirstOrDefault(i => i.Placement == Placement.Peek);

                // If we don't support both peek and background, then just peek is used
                if (!binding.SupportedFeatures.BackgroundAndPeekImage && backgroundImage != null && peekImage != null)
                {
                    backgroundImage = null;
                }

                // Calculate overlays
                double backgroundOverlay = 0;

                if (backgroundImage != null)
                {
                    if (backgroundImage.HintOverlay != null)
                    {
                        backgroundOverlay = backgroundImage.HintOverlay.Value;
                    }
                    else if (binding.HintOverlay != null)
                    {
                        backgroundOverlay = binding.HintOverlay.Value;
                    }
                    else
                    {
                        // If there's text on the tile, defaults to 20
                        backgroundOverlay = container.GetAllDescendants().OfType<AdaptiveTextField>().Any()
                            ? 20
                            // Else defaults to no overlay
                            : 0;
                    }

                    // If we're ignoring the dev specified background when there's no text on the tile
                    if (!binding.SupportedFeatures.RespectDevSpecifiedBackgroundOverlayEvenWhenNoTextOnTile)
                    {
                        if (!container.GetAllDescendants().OfType<AdaptiveTextField>().Any())
                        {
                            backgroundOverlay = 0;
                        }
                    }
                }

                double peekOverlay = 0;

                if (peekImage != null)
                {
                    peekOverlay = peekImage.HintOverlay != null
                        ? peekImage.HintOverlay.Value
                        // New in TH2: Binding overlay applies to both peek and background
                        : binding.HintOverlay != null && binding.SupportedFeatures.OverlayForBothBackgroundAndPeek
                        ? binding.HintOverlay.Value
                        // Defaults to 0
                        : 0;
                }


                if (backgroundImage != null)
                {
                    switch (backgroundImage.HintCrop)
                    {
                        case HintCrop.Circle:
                            BackgroundImageContainer.Child = new CircleImage()
                            {
                                Source = ImageHelper.GetBitmap(backgroundImage.Src),
                                Margin = tileSize == TileSize.Small ? new Thickness(4) : new Thickness(8),
                                OverlayOpacity = backgroundOverlay / 100.0
                            };
                            break;

                        default:
                            BackgroundImageContainer.Child = new Image()
                            {
                                Source = ImageHelper.GetBitmap(backgroundImage.Src),
                                Stretch = Stretch.UniformToFill
                            };
                            BackgroundImageOverlay.Opacity = backgroundOverlay / 100.0;
                            BackgroundImageOverlay.Visibility = Visibility.Visible;
                            break;
                    }
                }

                if (peekImage != null)
                {
                    UsingPeek = true;

                    switch (peekImage.HintCrop)
                    {
                        case HintCrop.Circle:
                            PeekImageContainer.Child = new CircleImage()
                            {
                                Source = ImageHelper.GetBitmap(peekImage.Src),
                                Margin = tileSize == TileSize.Small ? new Thickness(4) : new Thickness(8),
                                OverlayOpacity = peekOverlay / 100.0
                            };
                            break;

                        default:
                            PeekImageContainer.Child = new Grid()
                            {
                                Children =
                                {
                                    new Image()
                                    {
                                        Source = ImageHelper.GetBitmap(peekImage.Src),
                                        Stretch = Stretch.UniformToFill
                                    },
                                    
                                    // Overlay
                                    new Rectangle()
                                    {
                                        Fill = new SolidColorBrush(Colors.Black),
                                        Opacity = peekOverlay / 100.0
                                    }
                                }
                            };
                            break;
                    }

                    PeekRow.Height = new GridLength(1, GridUnitType.Star);
                }
            }
        }

        public bool UsingPeek { get; private set; }

        private double GetExternalMargin()
        {
            switch (_tileSize)
            {
                case TileSize.Small:
                    return AdaptiveConstants.SmallExternalMargin;

                default:
                    return AdaptiveConstants.DefaultExternalMargin;
            }
        }
    }
}
