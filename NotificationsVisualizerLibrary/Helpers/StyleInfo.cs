namespace NotificationsVisualizerLibrary.Helpers
{
    internal class StyleInfo
    {
        public double LineHeightOverride;
        public double MinLineHeight;
        public double FirstGroupMargin;
        public double TopOffset;
        public double[] TopMarginValues;

        public StyleInfo(double lineHeight, double minLineHeight, double firstGroupMargin, double topOffset, double[] topMarginValues)
        {
            LineHeightOverride = lineHeight;
            MinLineHeight = minLineHeight;
            FirstGroupMargin = firstGroupMargin;
            TopOffset = topOffset;
            TopMarginValues = topMarginValues;
        }
    }
}
