using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Model.TileElements;
using NotificationsVisualizerLibrary.Parsers;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TemplateVisualizerLibrary.Model.TileElements
{
    internal class TileVisual : Visual
    {
        private static readonly string ATTR_VISUAL_BRANDING = "branding";
        private static readonly string ATTR_VISUAL_DISPLAY_NAME = "displayName";

        public Branding? Branding { get; set; } = null;

        public string DisplayName { get; set; } = null;

        public IList<TileBinding> Bindings { get; set; } = new List<TileBinding>();

        public void Add(TileBinding element)
        {
            Bindings.Add(element);
            //element.Parent = this;
        }

        internal override void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            base.ParseKnownAttributes(attributes, result, baseUri, addImageQuery);

            // Branding is optional
            if (TryParseEnum(result, attributes, ATTR_VISUAL_BRANDING, out Branding branding, false)) // not case-sensitive
            { Branding = branding; }

            // DisplayName is optional
            XAttribute attrDisplayName = attributes.PopAttribute(ATTR_VISUAL_DISPLAY_NAME, false); // not case-sensitive
            if (attrDisplayName != null)
            { DisplayName = attrDisplayName.Value; }
        }

        protected override void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery)
        {
            if (child.IsType("binding"))
            {
                TileBinding binding = new TileBinding();
                binding.Parse(result, child, baseUri, addImageQuery);

                if (!result.IsOkForRender())
                { throw new IncompleteElementException(); }

                Add(binding);
            }
            else
            { result.AddWarning($"Invalid child {child.Name.LocalName} under visual element.", GetErrorPositionInfo(child)); }
        }
    }
}
