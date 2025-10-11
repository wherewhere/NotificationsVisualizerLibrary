using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary.Controls
{
    internal enum PeekContentDisplayed
    {
        PeekImage,
        Content
    }

    internal sealed partial class PeekDisplayerControl : UserControl
    {
        public PeekDisplayerControl() => InitializeComponent();

        #region PreviewTileNotificationRaw

        private static readonly DependencyProperty PreviewTileNotificationRawProperty =
            DependencyProperty.Register(
                nameof(PreviewTileNotificationRaw),
                typeof(PreviewTileNotificationRaw),
                typeof(PeekDisplayerControl),
                null);

        internal PreviewTileNotificationRaw PreviewTileNotificationRaw
        {
            get => (GetValue(PreviewTileNotificationRawProperty) as PreviewTileNotificationRaw)!;
            set => SetValue(PreviewTileNotificationRawProperty, value);
        }

        #endregion

        #region PeekStartsOn

        private static readonly DependencyProperty PeekStartsOnProperty =
            DependencyProperty.Register(
                nameof(PeekStartsOn),
                typeof(PeekContentDisplayed),
                typeof(PeekDisplayerControl),
                new PropertyMetadata(PeekContentDisplayed.PeekImage, OnPeekStartsOnPropertyChanged));

        private static void OnPeekStartsOnPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PeekDisplayerControl)!.OnPeekStartsOnPropertyChanged(e);
        }

        private void OnPeekStartsOnPropertyChanged(DependencyPropertyChangedEventArgs e) => InitializePeekAnimation();

        public PeekContentDisplayed PeekStartsOn
        {
            get => (PeekContentDisplayed)GetValue(PeekStartsOnProperty);
            set => SetValue(PeekStartsOnProperty, value);
        }

        #endregion

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size size = e.NewSize;

            if (size.IsEmpty || double.IsInfinity(size.Height) || double.IsInfinity(size.Width))
            {
                return;
            }

            // Set clipping so content outside the canvas isn't displayed
            CanvasClip.Rect = new Rect(new Point(), size);

            // Set the size of the content container
            NotificationContentContainer.Width = size.Width;
            NotificationContentContainer.Height = size.Height * 2; // Height gets doubled to support peek

            // And update the peek animation
            InitializePeekAnimation();
        }

        private static DoubleAnimationUsingKeyFrames GeneratePeekAnimation(double normalHeight, PeekContentDisplayed startingContent, TimeSpan timeBetweenSwitch)
        {
            double valueWhenPeekVisible = 0;
            double valueWhenContentVisible = normalHeight * -1;
            //double halfValue = normalHeight * -0.5;

            double initialValue;
            double switchedValue;

            TimeSpan durationOfSwitchAnimation = TimeSpan.FromSeconds(1.5);

            switch (startingContent)
            {
                case PeekContentDisplayed.PeekImage:
                    initialValue = valueWhenPeekVisible;
                    switchedValue = valueWhenContentVisible;
                    break;

                case PeekContentDisplayed.Content:
                    initialValue = valueWhenContentVisible;
                    switchedValue = valueWhenPeekVisible;
                    break;

                default:
                    throw new NotImplementedException();
            }

            EasingFunctionBase easingFunction = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseInOut,
                Exponent = 4
            };

            DoubleAnimationUsingKeyFrames a = new();

            // Initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = initialValue,
                KeyTime = new TimeSpan()
            });

            // Holding initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = initialValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(timeBetweenSwitch)
            });

            // Switched position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = switchedValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(durationOfSwitchAnimation),
                EasingFunction = easingFunction
            });

            // Holding switched position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = switchedValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(timeBetweenSwitch)
            });

            // Back to initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = initialValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(durationOfSwitchAnimation),
                EasingFunction = easingFunction
            });


            return a;
        }

        private Storyboard? _storyboard;

        [MemberNotNull(nameof(_storyboard))]
        private void InitializePeekAnimation()
        {
            double normalHeight = PresentationCanvas.ActualHeight;

            _storyboard?.Stop();

            _storyboard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            DoubleAnimationUsingKeyFrames a = GeneratePeekAnimation(
                normalHeight,
                PeekStartsOn,
                TimeSpan.FromSeconds(4));

            _storyboard.Children.Add(a);

            Storyboard.SetTarget(a, TranslateContent);
            Storyboard.SetTargetProperty(a, "Y");

            _storyboard.Begin();
        }
    }
}
