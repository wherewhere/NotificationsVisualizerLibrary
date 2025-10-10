using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model.TileElements
{
    internal class TileBinding : Binding
    {
        private static readonly string ATTR_BINDING_BRANDING = "branding";
        private static readonly string ATTR_BINDING_DISPLAY_NAME = "displayName";
        private static readonly string ATTR_BINDING_HINT_TEXT_STACKING = "hint-textStacking";
        private static readonly string ATTR_BINDING_HINT_OVERLAY = "hint-overlay";

        public Template Template { get; set; } = Template.Unsupported;

        public Branding? Branding { get; set; } = null;
        public HintTextStacking HintTextStacking { get; set; } = HintTextStacking.Top;

        private double _hintOverlay = 20;
        public double HintOverlay
        {
            get => _hintOverlay;
            set => _hintOverlay = value < 0 ? 0 : value > 100 ? 100 : value;
        }

        public string DisplayName { get; set; }

        public Image PeekImage { get; set; }

        public Image BackgroundImage { get; set; }

        public void Add(Group child)
        {
            AddHelper(child);
        }

        protected override void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            base.ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            // Template is required
            XAttribute attrTemplate = attributes.PopAttribute(ATTR_BINDING_TEMPLATE);
            if (attrTemplate == null)
            {
                result.AddWarning("template attribute wasn't provided on binding element.", GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            // If template is unknown, stop there
            if (!TryParseEnum(attrTemplate.Value, out Template template))
            {
                result.AddWarning($"template attribute \"{attrTemplate.Value}\" is not supported.", GetErrorPositionInfo(attrTemplate));
                throw new IncompleteElementException();
            }

            Template = template;

            // Branding is optional
            if (TryParseEnum(result, attributes, ATTR_BINDING_BRANDING, out Branding branding, false)) // not case-sensitive
            { Branding = branding; }

            // Display name is optional
            XAttribute attrDisplayName = attributes.PopAttribute(ATTR_BINDING_DISPLAY_NAME, false); // not case-sensitive
            if (attrDisplayName != null)
            { DisplayName = attrDisplayName.Value; }

            // hint-overlay is optional
            if (TryParse(result, attributes, ATTR_BINDING_HINT_OVERLAY, out double hintOverlay))
            { HintOverlay = hintOverlay; }

            // hint-textStacking is optional
            if (TryParseEnum(result, attributes, ATTR_BINDING_HINT_TEXT_STACKING, out HintTextStacking hintTextStacking))
            { HintTextStacking = hintTextStacking; }
        }

        protected override void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "text":

                    TileTextField text = new TileTextField();
                    text.Parse(result, child);

                    if (!result.IsOkForRender())
                    { return; }

                    Add(text);

                    break;

                case "image":

                    TileImage image = new TileImage();
                    image.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                    { return; }
                    
                    switch (image.Placement)
                    {
                        case Placement.Peek:

                            if (PeekImage != null)
                            { result.AddWarning("Multiple peek images were supplied inside a binding. Only the first will be used.", GetErrorPositionInfo(child)); }
                            else
                            { PeekImage = image; }

                            break;

                        case Placement.Background:
                            if (BackgroundImage != null)
                            { result.AddWarning("Multiple background images were supplied inside a binding. Only the first will be used.", GetErrorPositionInfo(child)); }
                            else
                            { BackgroundImage = image; }
                            break;

                        default:
                            Add(image);
                            break;
                    }

                    break;

                case "group":

                    TileGroup group = new TileGroup();
                    group.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                    { return; }

                    if (group != null)
                    { Add(group); }

                    break;

                default:
                    result.AddWarning($"Invalid child \"{child.Name.LocalName}\" under binding element. It will be ignored.", GetErrorPositionInfo(child));
                    break;
            }
        }
    }
}
