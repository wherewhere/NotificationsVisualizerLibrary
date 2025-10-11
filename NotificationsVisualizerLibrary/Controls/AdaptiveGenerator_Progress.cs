using NotificationsVisualizerLibrary.Model;
using Windows.UI.Xaml;

namespace NotificationsVisualizerLibrary.Controls
{
    internal static class AdaptiveGenerator_Progress
    {
        public static FrameworkElement Generate(AdaptiveProgress progress) => new ProgressView()
        {
            DataContext = progress
        };
    }
}
