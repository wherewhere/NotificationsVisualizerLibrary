using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal abstract class BaseAdaptiveElement(NotificationType context, FeatureSet supportedFeatures) : BaseElement
    {
        internal const string ATTR_ARGUMENTS = "arguments";
        internal const string ATTR_ACTIVATIONTYPE = "activationType";
        internal const string ATTR_STATE = "state";

        public NotificationType Context { get; protected set; } = context;

        public FeatureSet SupportedFeatures { get; private set; } = supportedFeatures;

        internal void ParseActivatableElementAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            IActivatableElement? el = this as IActivatableElement;

            // activationType is optional
            if (TryParseEnum(result, attributes, ATTR_ACTIVATIONTYPE, out ActivationType type))
            {
                el!.ActivationType = type;
            }

            // arguments is required
            XAttribute? attrArguments = attributes.PopAttribute(ATTR_ARGUMENTS);
            if (attrArguments == null)
            {
                // If we're in an activation type that requires attributes
                if (el!.ActivationType != ActivationType.None)
                {
                    result.AddErrorButRenderAllowed("arguments attribute is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                    throw new IncompleteElementException();
                }
            }
            else
            {
                el!.Arguments = attrArguments.Value;
            }
        }

        public virtual ObjectModelObject? ConvertToObject()
        {
            ObjectModelClassAttribute? classAttr = GetObjectModelClassAttribute();
            if (classAttr == null)
            {
                return null;
            }

            ObjectModelObject answer = new(classAttr.Name);

            foreach (PropertyInfo propInfo in GetType().GetProperties())
            {
                ObjectModelPropertyAttribute? propAttr = GetObjectModelPropertyAttribute(propInfo);
                if (propAttr != null)
                {
                    object? propVal = propInfo.GetValue(this);
                    if (!Equals(propAttr.DefaultValue, propVal))
                    {
                        IObjectModelValue? value = CreateObject(propVal);

                        if (value != null)
                        {
                            answer.Add(propAttr.Name, value);
                        }
                    }
                }
                else
                {
                    ObjectModelBindingPropertyAttribute? bindingPropAttr = GetObjectModelBindingPropertyAttribute(propInfo);
                    if (bindingPropAttr != null)
                    {
                        if (propInfo.GetValue(this) is string bindingName)
                        {
                            answer.AddBinding(bindingPropAttr, bindingName);
                        }
                    }
                }
            }

            return answer;
        }

        private IObjectModelValue? CreateObject(object? propVal)
        {
            switch (propVal)
            {
                case BaseAdaptiveElement element:
                    return element.ConvertToObject();

                case IEnumerable e and not string and not IObjectModelValue:
                    ObjectModelListInitialization listInit = new();
                    foreach (object o in e)
                    {
                        listInit.Add(CreateObject(o));
                    }
                    return listInit;

                case Enum @enum:
                    ObjectModelEnumAttribute? enumAttr = GetForContext(propVal.GetType().GetTypeInfo().GetCustomAttributes<ObjectModelEnumAttribute>());
                    if (enumAttr != null)
                    {
                        return new ObjectModelEnum(enumAttr.Name, @enum.ToString());
                    }
                    break;
            }

            return ObjectModelObject.Create(propVal);
        }

        private ObjectModelClassAttribute? GetObjectModelClassAttribute() =>
            GetForContext(GetType().GetTypeInfo().GetCustomAttributes<ObjectModelClassAttribute>());

        private ObjectModelPropertyAttribute? GetObjectModelPropertyAttribute(PropertyInfo propInfo) =>
            GetForContext(propInfo.GetCustomAttributes<ObjectModelPropertyAttribute>());

        private ObjectModelBindingPropertyAttribute? GetObjectModelBindingPropertyAttribute(PropertyInfo propInfo) =>
            GetForContext(propInfo.GetCustomAttributes<ObjectModelBindingPropertyAttribute>());

        private T? GetForContext<T>(IEnumerable<T> attributes) where T : ObjectModelBaseAttribute
        {
            T? matching = attributes.FirstOrDefault(i => i.Context != null && i.Context.Value == Context);
            return matching ?? attributes.FirstOrDefault(i => i.Context == null);
        }
    }
}
