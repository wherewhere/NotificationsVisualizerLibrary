using System.Collections.Generic;
using System.Linq;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStylesMap
    {
        private readonly List<TextStyleInfoPair> _pairs = new List<TextStyleInfoPair>();

        public void Add(TextStyleInfoPair pair)
        {
            _pairs.Add(pair);
        }

        public TextStyleInfo Find(string styleName)
        {
            return _pairs.FirstOrDefault(i => i.StyleName.Equals(styleName))?.TextStyleInfo;
        }
    }
}
