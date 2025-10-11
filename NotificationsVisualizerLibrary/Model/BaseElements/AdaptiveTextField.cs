using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveText")]
    internal partial class AdaptiveTextField(NotificationType context, FeatureSet supportedFeatures) : AdaptiveChildElement(context, supportedFeatures), IBindingChild
    {
        private const string ATTR_TEXT_ID = "id";
        private const string ATTR_TEXT_LANG = "lang";
        private const string ATTR_TEXT_HINT_STYLE = "hint-style";
        private const string ATTR_TEXT_HINT_WRAP = "hint-wrap";
        private const string ATTR_TEXT_HINT_MAX_LINES = "hint-maxLines";
        private const string ATTR_TEXT_HINT_MIN_LINES = "hint-minLines";
        private const string ATTR_TEXT_HINT_ALIGN = "hint-align";
        private const string ATTR_TEXT_PLACEMENT = "placement";

        public string? Id { get; set; }

        public string Text
        {
            get;
            set { SetProperty(ref field, value); }
        } = "";

        [ObjectModelBindingProperty(nameof(Text), "BindableString")]
        public string? BindingText { get; set; }

        [ObjectModelProperty("Text", null)]
        public IObjectModelValue? PropertyText => BindingText != null ? null : Text == null ? null : new ObjectModelString(Text);

        [ObjectModelProperty("Language")]
        public string? Language { get; set; }

        public TextPlacement Placement { get; set; }

        [ObjectModelProperty("HintStyle", HintStyle.Default)]
        public HintStyle HintStyle { get; set; } = HintStyle.Default;

        [ObjectModelProperty("HintWrap")]
        public bool? HintWrap { get; set; }

        [ObjectModelProperty("HintMaxLines")]
        public int? HintMaxLines
        {
            get;
            set => field = value != null && value.Value < 1 ? 1 : value;
        }

        [ObjectModelProperty("HintMinLines")]
        public int? HintMinLines
        {
            get;
            set => field = value < 1 ? 1 : value > 10 ? 10 : value;
        }

        [ObjectModelProperty("HintAlign", HintAlign.Default)]
        public HintAlign HintAlign { get; set; } = HintAlign.Default;

        internal void Parse(ParseResult result, XElement node, bool isBindingRootLevel)
        {
            // Inner text
            // Binding only supported on top level toast elements
            if (isBindingRootLevel && Context == NotificationType.Toast && SupportedFeatures.ToastTextDataBinding)
            {
                TryProcessBindableValue(result, node.Value, out string? bindingText, val =>
                {
                    Text = val;
                });
                BindingText = bindingText;
            }
            else
            {
                Text = node.Value;
            }

            AttributesHelper attributes = new(node.Attributes());

            ParseKnownAttributes(attributes, result, isBindingRootLevel);

            HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, bool isBindingRootLevel)
        {
            // Max lines is supported on non-toasts, and adaptive toasts, and group/subgroups
            if (Context != NotificationType.Toast || SupportedFeatures.AdaptiveToasts || !isBindingRootLevel)
            {
                // hint-max-lines is optional
                if (TryParse(result, attributes, ATTR_TEXT_HINT_MAX_LINES, out int hintMaxLines))
                {
                    HintMaxLines = hintMaxLines;
                }
            }

            // These features are supported on non-toasts, and group/subgroups
            if (Context != NotificationType.Toast || !isBindingRootLevel)
            {
                // hint-align is optional
                if (TryParseEnum(result, attributes, ATTR_TEXT_HINT_ALIGN, out HintAlign hintAlign))
                {
                    HintAlign = hintAlign;
                }

                // hint-min-lines is optional
                if (TryParse(result, attributes, ATTR_TEXT_HINT_MIN_LINES, out int hintMinLines))
                {
                    HintMinLines = hintMinLines;
                }

                // hint-style is optional
                if (TryParseEnum(result, attributes, ATTR_TEXT_HINT_STYLE, out HintStyle hintStyle))
                {
                    HintStyle = hintStyle;
                }

                // hint-wrap is optional
                if (TryParse(result, attributes, ATTR_TEXT_HINT_WRAP, out bool hintWrap))
                {
                    HintWrap = hintWrap;
                }
            }

            if (Context == NotificationType.Tile)
            {
                Id = attributes.PopAttributeValue(ATTR_TEXT_ID);
            }

            if (TryParseEnum(result, attributes, ATTR_TEXT_PLACEMENT, out TextPlacement placement))
            {
                Placement = placement;
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer() => (string[])[ATTR_TEXT_LANG];

        protected override TEnum[] GetSupportedEnums<TEnum>()
        {
            // Override the allowed text placement values, since they depend on OS version / supported features
            if (typeof(TEnum) == typeof(TextPlacement))
            {
                switch (Context)
                {
                    case NotificationType.Toast:

                        if (SupportedFeatures.ToastAttribution)
                        {
                            return (new TextPlacement[] { TextPlacement.Inline, TextPlacement.Attribution } as TEnum[])!;
                        }

                        return (new TextPlacement[] { TextPlacement.Inline } as TEnum[])!;

                    default:
                        return (new TextPlacement[] { TextPlacement.Inline } as TEnum[])!;
                }
            }

            return base.GetSupportedEnums<TEnum>();
        }

        public override ObjectModelObject? ConvertToObject() => Placement switch
        {
            TextPlacement.Attribution => new ObjectModelObject("ToastGenericAttributionText")
            {
                { nameof(Text), Text },
                { nameof(Language), Language }
            },
            _ => base.ConvertToObject(),
        };
    }
}
