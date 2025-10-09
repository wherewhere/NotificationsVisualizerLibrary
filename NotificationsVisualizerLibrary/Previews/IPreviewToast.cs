using NotificationsVisualizerLibrary.Parsers;
using Windows.Data.Xml.Dom;

namespace NotificationsVisualizerLibrary
{
    public interface IPreviewToast
    {
        ParseResult Initialize(XmlDocument content);
        ParseResult Initialize(XmlDocument content, PreviewNotificationData data);
        void Update(PreviewNotificationData data);
        PreviewToastProperties Properties { get; set; }
        int OSBuildNumber { get; set; }
    }
}
