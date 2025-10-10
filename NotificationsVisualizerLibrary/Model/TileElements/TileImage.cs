using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model.TileElements
{
    internal class TileImage : Image
    {
        private static readonly string ATTR_IMAGE_PLACEMENT = "placement";
        private static readonly string ATTR_IMAGE_HINT_REMOVE_MARGIN = "hint-removeMargin";
        private static readonly string ATTR_IMAGE_HINT_ALIGN = "hint-align";

        public bool HintRemoveMargin { get; set; } = false;

        public HintImageAlign HintAlign { get; set; } = HintImageAlign.Stretch;

        public Placement Placement { get; set; } = Placement.Inline;

        internal override void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            base.ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            // placement defaults to inline
            if (TryParseEnum(result, attributes, ATTR_IMAGE_PLACEMENT, out Placement placement))
            {
                Placement = placement;
            }

            // hint-removeMargin is optional
            if (TryParse(result, attributes, ATTR_IMAGE_HINT_REMOVE_MARGIN, out bool hintRemoveMargin))
            { HintRemoveMargin = hintRemoveMargin; }

            // hint-align is optional
            if (TryParseEnum(result, attributes, ATTR_IMAGE_HINT_ALIGN, out HintImageAlign hintAlign))
            { HintAlign = hintAlign; }
        }
    }
}
