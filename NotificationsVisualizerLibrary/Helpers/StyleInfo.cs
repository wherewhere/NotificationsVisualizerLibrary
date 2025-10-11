namespace NotificationsVisualizerLibrary.Helpers
{
    internal class StyleInfo(double lineHeight, double minLineHeight, double firstGroupMargin, double topOffset, double[] topMarginValues)
    {
        public double LineHeightOverride = lineHeight;
        public double MinLineHeight = minLineHeight;
        public double FirstGroupMargin = firstGroupMargin;
        public double TopOffset = topOffset;
        public double[] TopMarginValues = topMarginValues;
    }
}
