using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveProgressBar")]
    internal partial class AdaptiveProgress(NotificationType context, FeatureSet supportedFeatures) : AdaptiveChildElement(context, supportedFeatures), IBindingChild
    {
        private const string ATTR_TITLE = "title";
        private const string ATTR_VALUE_STRING_OVERRIDE = "valueStringOverride";
        private const string ATTR_VALUE = "value";
        private const string ATTR_STATUS = "status";

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer() => (string[])[];

        /// <summary>
        /// Optional
        /// </summary>
        public string? Title
        {
            get;
            set => SetProperty(ref field, value);
        }

        [ObjectModelBindingProperty(nameof(Value), "BindableProgressBarValue")]
        public string? BindingValue { get; set; }

        [ObjectModelBindingProperty(nameof(ValueStringOverride), "BindableString")]
        public string? BindingValueStringOverride { get; set; }

        [ObjectModelBindingProperty(nameof(Title), "BindableString")]
        public string? BindingTitle { get; set; }

        [ObjectModelBindingProperty(nameof(Status), "BindableString")]
        public string? BindingStatus { get; set; }

        [ObjectModelProperty("Value", null)]
        public IObjectModelValue? PropertyValue
        {
            get
            {
                if (BindingValue != null)
                {
                    // If data binding being used, the other property will pick it up
                    return null;
                }

                return IsIndeterminate
                    ? new ObjectModelEnum("AdaptiveProgressBarValue", "Intermediate")
                    : new ObjectModelObjectFromStaticMethod("AdaptiveProgressBarValue", "FromValue", new ObjectModelDouble(Value));
            }
        }

        [ObjectModelProperty("ValueStringOverride", null)]
        public IObjectModelValue? PropertyValueStringOverride => BindingValueStringOverride != null ? null : ValueStringOverride == null ? null : new ObjectModelString(ValueStringOverride);

        [ObjectModelProperty("Title", null)]
        public IObjectModelValue? PropertyTitle => BindingTitle != null ? null : Title == null ? null : new ObjectModelString(Title);

        [ObjectModelProperty("Status", null)]
        public IObjectModelValue? PropertyStatus => BindingStatus != null ? null : Status == null ? null : new ObjectModelString(Status);

        /// <summary>
        /// Required, value between 0.0 and 1.0
        /// </summary>
        public double Value
        {
            get;
            set { SetProperty(ref field, value); NotifyPropertyChanged(nameof(ValueString)); }
        }

        public string ValueString => ValueStringOverride ?? Value.ToString("0%");

        public bool IsIndeterminate
        {
            get;
            set => SetProperty(ref field, value);
        }

        public string? ValueStringOverride
        {
            get;
            set { SetProperty(ref field, value); NotifyPropertyChanged(nameof(ValueString)); }
        }

        /// <summary>
        /// Optional
        /// </summary>
        public string? Status
        {
            get;
            set => SetProperty(ref field, value);
        }

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // title is optional
            TryPopAttributeValueWithBinding(result, attributes, ATTR_TITLE, out string? bindingTitle, (val) =>
            {
                Title = val;
            });
            BindingTitle = bindingTitle;

            // valueStringOverride is optional
            TryPopAttributeValueWithBinding(result, attributes, ATTR_VALUE_STRING_OVERRIDE, out string? bindingValueStringOverride, val =>
            {
                ValueStringOverride = val;
            });
            BindingValueStringOverride = bindingValueStringOverride;

            // value is required
            bool hadValue = TryPopAttributeValueWithBinding(result, attributes, ATTR_VALUE, out string? bindingValue, val =>
            {
                if (val == null)
                {
                    throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value wasn't provided.", XmlTemplateParser.GetErrorPositionInfo(node)));
                }

                double value = 0;
                if (val.Equals("indeterminate", StringComparison.CurrentCultureIgnoreCase))
                {
                    IsIndeterminate = true;
                }
                else if (!double.TryParse(val, out value))
                {
                    throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value must be a double between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node)));
                }
                else
                {
                    if (value is < 0 or > 1)
                    {
                        IsIndeterminate = false;
                        Value = (value < 0) ? 0 : 1;
                        throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value must be between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node)));
                    }

                    IsIndeterminate = false;
                    Value = value;
                }
            });
            if (!hadValue)
            {
                Value = 0;
                result.AddErrorButRenderAllowed("value attribute on progress element is required and must be a double between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node));
            }

            BindingValue = bindingValue;

            // status is optional
            TryPopAttributeValueWithBinding(result, attributes, ATTR_STATUS, out string? bindingStatus, val =>
            {
                Status = val;
            });
            BindingStatus = bindingStatus;
        }
    }
}
