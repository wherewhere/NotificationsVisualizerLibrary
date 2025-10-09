using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveImage")]
    internal class AdaptiveImage : AdaptiveChildElement, IBindingChild
    {
        public AdaptiveImage(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private const string ATTR_IMAGE_SRC = "src";
        private const string ATTR_IMAGE_ALT = "alt";
        private const string ATTR_IMAGE_ADDIMAGEQUERY = "addImageQuery";
        private const string ATTR_IMAGE_HINT_CROP = "hint-crop";
        private const string ATTR_IMAGE_PLACEMENT = "placement";
        private const string ATTR_IMAGE_HINT_REMOVE_MARGIN = "hint-removeMargin";
        private const string ATTR_IMAGE_HINT_ALIGN = "hint-align";
        private const string ATTR_HINT_OVERLAY = "hint-overlay";

        public string Id { get; set; }

        [ObjectModelProperty("AlternateText")]
        public string AlternateText { get; set; }

        [ObjectModelProperty("AddImageQuery")]
        public bool? AddImageQuery { get; set; }

        [ObjectModelProperty("HintCrop", HintCrop.Default)]
        public HintCrop HintCrop { get; set; } = HintCrop.Default;

        [ObjectModelProperty("HintRemoveMargin")]
        public bool? HintRemoveMargin { get; set; }

        [ObjectModelProperty("HintAlign", HintImageAlign.Default)]
        public HintImageAlign HintAlign { get; set; } = HintImageAlign.Default;

        public Placement Placement { get; set; } = Placement.Inline;

        private double? _hintOverlay = null;
        public double? HintOverlay
        {
            get => _hintOverlay;
            set
            {
                if (value != null)
                {
                    _hintOverlay = value.Value < 0 ? (double?)0 : value.Value > 100 ? (double?)100 : value;
                }
                else
                {
                    _hintOverlay = value;
                }
            }
        }

        /// <summary>
        /// Required
        /// </summary>
        [ObjectModelProperty("Source")]
        public string Src { get; set; }

        internal void Parse(ParseResult result, XElement node, string baseUri, bool addImageQuery)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            // AddImageQuery is optional
            {
                if (TryParse(result, attributes, ATTR_IMAGE_ADDIMAGEQUERY, out bool val))
                {
                    addImageQuery = val; // Overwrite cascaded value if it was specified
                    AddImageQuery = val;
                }
            }

            // src is required
            XAttribute attrSrc = attributes.PopAttribute(ATTR_IMAGE_SRC);
            if (attrSrc == null)
            {
                result.AddError("src attribute on image element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            Src = XmlTemplateParser.ConstructUri(attrSrc.Value, baseUri, addImageQuery);

            // alt is optional, I don't use it right now
            XAttribute altAttr = attributes.PopAttribute(ATTR_IMAGE_ALT);
            if (altAttr != null)
            {
                AlternateText = altAttr.Value;
            }

            // placement defaults to inline
            if (TryParseEnum(result, attributes, ATTR_IMAGE_PLACEMENT, out Placement placement))
            {
                Placement = placement;
            }

            switch (placement)
            {
                case Placement.Background:

                    if (SupportedFeatures.CropCircleOnBackgroundImage)
                    {
                        HandleHintCrop(result, attributes);
                    }

                    if (SupportedFeatures.OverlayForBothBackgroundAndPeek)
                    {
                        HandleHintOverlay(result, attributes);
                    }

                    break;

                case Placement.Peek:

                    if (SupportedFeatures.CropCircleOnPeekImage)
                    {
                        HandleHintCrop(result, attributes);
                    }

                    if (SupportedFeatures.OverlayForBothBackgroundAndPeek)
                    {
                        HandleHintOverlay(result, attributes);
                    }

                    break;

                case Placement.Hero:
                    break;

                default:
                    HandleHintCrop(result, attributes);

                    // These only apply to tiles, and only to inline images
                    if (Context != NotificationType.Toast
                        || SupportedFeatures.AdaptiveToasts
                        )
                    {
                        // hint-removeMargin is optional
                        if (TryParse(result, attributes, ATTR_IMAGE_HINT_REMOVE_MARGIN, out bool hintRemoveMargin))
                        {
                            HintRemoveMargin = hintRemoveMargin;
                        }

                        // hint-align is optional
                        if (TryParseEnum(result, attributes, ATTR_IMAGE_HINT_ALIGN, out HintImageAlign hintAlign))
                        {
                            HintAlign = hintAlign;
                        }
                    }

                    break;
            }
        }

        private void HandleHintOverlay(ParseResult result, AttributesHelper attributes)
        {
            // hint-overlay is optional
            if (TryParse(result, attributes, ATTR_HINT_OVERLAY, out double hintOverlay))
            {
                HintOverlay = hintOverlay;
            }
        }

        private void HandleHintCrop(ParseResult result, AttributesHelper attributes)
        {
            if (TryParseEnum(result, attributes, ATTR_IMAGE_HINT_CROP, out HintCrop hintCrop))
            {
                HintCrop = hintCrop;
            }
        }

        protected override Array GetSupportedEnums<TEnum>()
        {
            if (typeof(TEnum) == typeof(Placement))
            {
                switch (Context)
                {
                    case NotificationType.Toast:

                        if (SupportedFeatures.RS1_Style_Toasts)
                        {
                            return new Placement[] { Placement.Inline, Placement.AppLogoOverride, Placement.Hero };
                        }

                        return new Placement[] { Placement.Inline, Placement.AppLogoOverride };

                    case NotificationType.Tile:
                        return new Placement[] { Placement.Inline, Placement.Background, Placement.Peek };
                }
            }

            return base.GetSupportedEnums<TEnum>();
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        public override ObjectModelObject ConvertToObject()
        {
            switch (Placement)
            {
                case Placement.Hero:
                    return new ObjectModelObject("ToastGenericHeroImage")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery }
                    };

                case Placement.AppLogoOverride:
                    return new ObjectModelObject("ToastGenericAppLogo")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery },
                        { "HintCrop", HintCrop != HintCrop.Default ? new ObjectModelEnum("ToastGenericAppLogoCrop", HintCrop.ToString()) : null }
                    };

                case Placement.Peek:
                    return new ObjectModelObject("TilePeekImage")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery },
                        { "HintOverlay", HintOverlay },
                        { "HintCrop", HintCrop != HintCrop.Default ? new ObjectModelEnum("TilePeekImageCrop", HintCrop.ToString()) : null }
                    };

                case Placement.Background:
                    return new ObjectModelObject("TileBackgroundImage")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery },
                        { "HintOverlay", HintOverlay },
                        { "HintCrop", HintCrop != HintCrop.Default ? new ObjectModelEnum("TileBackgroundImageCrop", HintCrop.ToString()) : null }
                    };

                default:
                    return base.ConvertToObject();
            }
        }
    }
}
