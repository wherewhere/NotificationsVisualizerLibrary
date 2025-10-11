using NotificationsVisualizerLibrary.Parsers;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastSelectionBoxItem", NotificationType.Toast)]
    internal partial class Selection(NotificationType context, FeatureSet supportedFeatures) : AdaptiveChildElement(context, supportedFeatures)
    {
        internal const string ATTR_ID = "id";
        internal const string ATTR_CONTENT = "content";

        public string? Id { get; set; }

        public string? Content { get; set; }

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        protected virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // id is required
            XAttribute? attrId = attributes.PopAttribute(ATTR_ID);
            if (attrId == null)
            {
                result.AddError("id attribute on input element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            // type is required
            XAttribute? attrContent = attributes.PopAttribute(ATTR_CONTENT);
            if (attrContent == null)
            {
                result.AddErrorButRenderAllowed("content attribute on input element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            Id = attrId.Value;
            Content = attrContent.Value;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer() => (string[])[];

        public override string? ToString() => Content;

        public override ObjectModelObject ConvertToObject()
        {
            ObjectModelObject obj = base.ConvertToObject()!;
            obj.ConstructorValues.Add(new ObjectModelString(Id!));
            obj.ConstructorValues.Add(new ObjectModelString(Content!));
            return obj;
        }
    }
}
