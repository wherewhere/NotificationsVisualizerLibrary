namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStyleInfoPair
    {
        public string StyleName { get; private set; }
        public TextStyleInfo TextStyleInfo { get; private set; }

        public TextStyleInfoPair(string styleName, TextStyleInfo textStyleInfo)
        {
            StyleName = styleName;
            TextStyleInfo = textStyleInfo;
        }
    }
}
