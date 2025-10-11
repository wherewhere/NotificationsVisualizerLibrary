using System;

namespace NotificationsVisualizerLibrary.Model.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class XmlValueAttribute(string xmlValue) : Attribute
    {
        public string XmlValue { get; private set; } = xmlValue;
    }
}
