using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NotificationsVisualizerLibraryTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Frame.Navigated += On_Navigated;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Frame.Navigated -= On_Navigated;
        }

        private void ButtonOpenSampleSettingsPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SampleSettingsPage));
        }

        private void ButtonOpenUpdateImageInUsePage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UpdateImageInUsePage));
        }

        private void ButtonUpdateLocalImageFrequently_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UpdateTileUsingLocalImagesFrequently));
        }
        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = TryGoBack();
        }

        private void System_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack() == AppViewBackButtonVisibility.Visible;
            }
        }

        private AppViewBackButtonVisibility TryGoBack(bool goBack = true)
        {
            if (!Dispatcher.HasThreadAccess || !Frame.CanGoBack)
            { return AppViewBackButtonVisibility.Collapsed; }

            if (goBack) { Frame.GoBack(); }
            return AppViewBackButtonVisibility.Visible;
        }
    }
}
