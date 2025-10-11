using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveSubgroup")]
    internal partial class AdaptiveSubgroup(NotificationType context, FeatureSet supportedFeatures) : AdaptiveParentElement(context, supportedFeatures)
    {
        private const string ATTR_SUBGROUP_HINT_WEIGHT = "hint-weight";
        private const string ATTR_SUBGROUP_HINT_TEXT_STACKING = "hint-textStacking";
        public const string ATTR_ACTIONID = "actionId";

        internal const int WEIGHT_AUTO = int.MinValue;

        /// <summary>
        /// Auto is WEIGHT_AUTO
        /// </summary>
        [ObjectModelProperty("HintWeight")]
        public int? HintWeight
        {
            get;
            set => field = value switch
            {
                null => null,
                //< 0 => 0,
                //> 100 => 100,
                _ => value,
            };
        }

        public string? ActionId { get; set; }

        [ObjectModelProperty("Children")]
        public IList<AdaptiveChildElement> Children { get; private set; } = [];

        [ObjectModelProperty("HintTextStacking", HintTextStacking.Default)]
        public HintTextStacking HintTextStacking { get; set; } = HintTextStacking.Default;

        public void Add(AdaptiveTextField child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public void Add(AdaptiveImage child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren() => Children;

        internal override AdaptiveChildElement? GetNextElementBelowThisOne() => Parent!.GetNextElementBelowThisOne();

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer() => (string[])[];

        internal void Parse(ParseResult result, XElement node, string? baseUri, bool addImageQuery)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
            {
                throw new IncompleteElementException();
            }

            AttributesHelper attributes = new(node.Attributes());

            ParseKnownAttributes(attributes, result, baseUri, addImageQuery);

            HandleRemainingAttributes(attributes, result);

            // Children of subgroup
            foreach (XElement n in node.Elements())
            {
                try
                {
                    HandleChild(result, n, baseUri, addImageQuery);
                }
                catch (IncompleteElementException) { }
            }
        }

        protected void HandleChild(ParseResult result, XElement child, string? baseUri, bool addImageQuery)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "text":

                    AdaptiveTextField text = new(Context, SupportedFeatures);
                    text.Parse(result, child, false);

                    if (!result.IsOkForRender())
                    {
                        throw new IncompleteElementException();
                    }

                    if (text != null)
                    {
                        Add(text);
                    }

                    break;

                case "image":

                    AdaptiveImage image = new(Context, SupportedFeatures);
                    image.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                    {
                        throw new IncompleteElementException();
                    }

                    if (image != null)
                    {
                        if (image.Placement != Placement.Inline)
                        {
                            result.AddErrorButRenderAllowed("You cannot place a non-inline image element inside a subgroup.", GetErrorPositionInfo(child));
                        }

                        Add(image);
                    }

                    break;

                default:
                    result.AddWarning($"Invalid child \"{child.Name.LocalName}\" under subgroup element. It will be ignored.", GetErrorPositionInfo(child));
                    break;
            }
        }

        internal virtual void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, string? baseUri, bool addImageQuery)
        {
            // hint-weight
            ParseHintWeight(attributes, result);

            // hint-textStacking is optional
            if (TryParseEnum(result, attributes, ATTR_SUBGROUP_HINT_TEXT_STACKING, out HintTextStacking hintTextStacking))
            {
                HintTextStacking = hintTextStacking;
            }

            if (Context is not NotificationType.Tile and not NotificationType.Toast)
            {
                // actionId is optional
                XAttribute? attrActionId = attributes.PopAttribute(ATTR_ACTIONID);
                if (attrActionId != null)
                {
                    ActionId = attrActionId.Value;
                }
            }
        }

        private void ParseHintWeight(AttributesHelper attributes, ParseResult result)
        {
            if (TryParse(result, attributes, ATTR_SUBGROUP_HINT_WEIGHT, out int hintWeight))
            {
                HintWeight = hintWeight;
            }
        }
    }
}