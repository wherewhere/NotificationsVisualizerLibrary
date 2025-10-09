using System;

namespace NotificationsVisualizerLibrary.Model.Enums
{
    internal class XmlValueAttribute : Attribute
    {
        public string XmlValue { get; private set; }

        public XmlValueAttribute(string xmlValue)
        {
            XmlValue = xmlValue;
        }
    }
}
