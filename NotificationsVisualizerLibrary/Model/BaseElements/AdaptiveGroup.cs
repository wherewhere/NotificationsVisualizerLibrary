using NotificationsVisualizerLibrary.Parsers;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveGroup")]
    internal partial class AdaptiveGroup(NotificationType context, FeatureSet supportedFeatures) : AdaptiveParentElement(context, supportedFeatures), IBindingChild
    {
        public const string ATTR_ACTIONID = "actionId";

        [ObjectModelProperty("Children")]
        public IList<AdaptiveSubgroup> Subgroups { get; private set; } = [];

        public string? ActionId { get; set; }

        public void Add(AdaptiveSubgroup element)
        {
            Subgroups.Add(element);
            element.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren() => Subgroups;

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

            foreach (XElement n in node.Elements())
            {
                try
                {
                    HandleChild(result, n, baseUri, addImageQuery);
                }
                catch (IncompleteElementException) { }
            }
        }

        internal void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, string? baseUri, bool addImageQuery)
        {
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

        protected void HandleChild(ParseResult result, XElement child, string? baseUri, bool addImageQuery)
        {
            if (child.IsType("subgroup"))
            {
                AdaptiveSubgroup subgroup = new(Context, SupportedFeatures);
                subgroup.Parse(result, child, baseUri, addImageQuery);

                if (!result.IsOkForRender())
                {
                    throw new IncompleteElementException();
                }

                if (subgroup != null)
                {
                    Add(subgroup);
                }
            }
            else
            {
                result.AddError($@"Invalid child ""{child.Name.LocalName}"" found in a group. Groups can only contain subgroups.", GetErrorPositionInfo(child));
            }
        }
    }
}
