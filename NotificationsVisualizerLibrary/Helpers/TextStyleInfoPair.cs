namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStyleInfoPair(string styleName, TextStyleInfo textStyleInfo)
    {
        public string StyleName { get; private set; } = styleName;
        public TextStyleInfo TextStyleInfo { get; private set; } = textStyleInfo;
    }
}
