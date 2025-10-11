using Windows.UI.Xaml;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStyleInfo(string xamlName, int index, TextLineBounds textLineBounds = TextLineBounds.Full)
    {
        public string XamlName { get; private set; } = xamlName;
        public int Index { get; private set; } = index;
        public TextLineBounds TextLineBounds { get; private set; } = textLineBounds;
    }
}
