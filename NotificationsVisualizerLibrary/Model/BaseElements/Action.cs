using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastButton", NotificationType.Toast)]
    internal partial class Action(NotificationType context, FeatureSet supportedFeatures) : AdaptiveChildElement(context, supportedFeatures), IActivatableElement
    {
        internal const string ATTR_CONTENT = "content";
        internal const string ATTR_IMAGEURI = "imageUri";
        internal const string ATTR_HINT_INPUTID = "hint-inputId";
        internal const string ATTR_PLACEMENT = "placement";

        public string? Id { get; set; }

        public ActionPlacement Placement { get; set; }

        public string? Content { get; set; }

        public string? Arguments { get; set; }

        [ObjectModelProperty("ActivationType")]
        public ActivationType ActivationType { get; set; } = ActivationType.Foreground;

        [ObjectModelProperty("ImageUri")]
        public string? ImageUri { get; set; }

        [ObjectModelProperty("InputId")]
        public string? InputId { get; set; }

        public bool HintVisible { get; set; } = true;

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer() => (string[])[];

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        [MemberNotNull(nameof(Content))]
        internal virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // content is required
            XAttribute? attrContent = attributes.PopAttribute(ATTR_CONTENT);
            if (attrContent == null)
            {
                result.AddError("content attribute on action element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            Content = attrContent.Value;

            // parse activationType, arguments, state
            ParseActivatableElementAttributes(node, attributes, result);

            // imageUri is optional
            XAttribute? attrImageUri = attributes.PopAttribute(ATTR_IMAGEURI);
            if (attrImageUri != null)
            {
                ImageUri = attrImageUri.Value;
            }

            // inputId is optional
            XAttribute? attrInputId = attributes.PopAttribute(ATTR_HINT_INPUTID);
            if (attrInputId != null)
            {
                InputId = attrInputId.Value;
            }

            if (TryParseEnum(result, attributes, ATTR_PLACEMENT, out ActionPlacement placement))
            {
                Placement = placement;
            }
        }

        protected override TEnum[] GetSupportedEnums<TEnum>()
        {
            // Override the allowed text placement values, since they depend on OS version / supported features
            if (typeof(TEnum) == typeof(ActionPlacement))
            {
                switch (Context)
                {
                    case NotificationType.Toast:

                        if (SupportedFeatures.ToastContextMenu)
                        {
                            return (new ActionPlacement[] { ActionPlacement.Inline, ActionPlacement.ContextMenu } as TEnum[])!;
                        }

                        return (new ActionPlacement[] { ActionPlacement.Inline } as TEnum[])!;

                    default:
                        return (new ActionPlacement[] { ActionPlacement.Inline } as TEnum[])!;
                }
            }

            return base.GetSupportedEnums<TEnum>();
        }

        public override ObjectModelObject ConvertToObject()
        {
            ObjectModelObject obj = base.ConvertToObject()!;

            obj.ConstructorValues.Add(new ObjectModelString(Content!));
            obj.ConstructorValues.Add(new ObjectModelString(Arguments!));

            switch (Placement)
            {
                case ActionPlacement.ContextMenu:
                    obj.Name = "ToastContextMenuItem";
                    break;

                default:
                    switch (ActivationType)
                    {
                        case ActivationType.System:
                            obj.PropertyValues.Clear();
                            string args = Arguments ?? "";
                            switch (args.ToLower())
                            {
                                case "snooze":
                                    obj = new ObjectModelObject("ToastButtonSnooze");
                                    if (!string.IsNullOrWhiteSpace(Content))
                                    {
                                        obj.ConstructorValues.Add(new ObjectModelString(Content!));
                                    }
                                    if (InputId != null)
                                    {
                                        obj.Add("SelectionBoxId", new ObjectModelString(InputId));
                                    }
                                    break;

                                default:
                                    obj = new ObjectModelObject("ToastButtonDismiss");
                                    if (!string.IsNullOrWhiteSpace(Content))
                                    {
                                        obj.ConstructorValues.Add(new ObjectModelString(Content!));
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }

            return obj;
        }
    }
}
