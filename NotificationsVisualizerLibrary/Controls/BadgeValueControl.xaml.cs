using NotificationsVisualizerLibrary.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary.Controls
{
    public sealed partial class BadgeValueControl : UserControl
    {
        public BadgeValueControl() => InitializeComponent();

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(BadgeValue),
                typeof(BadgeValueControl),
                new PropertyMetadata(BadgeValue.Default()));

        public BadgeValue Value
        {
            get => (GetValue(ValueProperty) as BadgeValue)!;
            set => SetValue(ValueProperty, value);
        }
    }
}
