using NotificationsVisualizerLibrary;
using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NotificationsVisualizerLibraryTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateTileUsingLocalImagesFrequently : Page
    {
        public UpdateTileUsingLocalImagesFrequently() => InitializeComponent();

        private async void ButtonUpdateImage_Click(object sender, RoutedEventArgs e)
        {
            ButtonUpdateImage.IsEnabled = false;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/PlannerAssets/Wide310x150Logo.scale-100.png"));
            await file.CopyAsync(ApplicationData.Current.LocalFolder, "background.png", NameCollisionOption.ReplaceExisting);

            string xml =
                "<tile> <visual>" +
                "<binding template=\"TileSmall\" branding=\"name\" displayName=\"Wednesday 31\"> <text>small {0}</text> </binding> " +
                "<binding template=\"TileMedium\" branding=\"name\" displayName=\"Wednesday 31\"> <text>medium {0}</text> </binding> " +
                "<binding template=\"TileWide\" branding=\"name\" displayName=\"Wednesday 31\">   <image hint-overlay=\"{1}\"  src=\"ms-appdata:///local/background.png\" placement=\"background\"/>       <text>wide {0}</text> </binding> " +
                "</visual> </tile>";

            PreviewTileUpdater updater = WidePreviewTile.CreateTileUpdater();

            for (int i = 0; i < 100; i++)
            {
                string tileXml = string.Format(xml, i, i % 100);

                XmlDocument xmlDoc = new();
                xmlDoc.LoadXml(tileXml);

                TileNotification notification = new(xmlDoc);

                updater.Update(notification);

                await Task.Delay(100);
            }

            ButtonUpdateImage.IsEnabled = true;
        }
    }
}
