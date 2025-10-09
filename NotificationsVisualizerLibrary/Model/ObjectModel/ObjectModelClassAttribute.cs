using System;

namespace NotificationsVisualizerLibrary.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ObjectModelClassAttribute : ObjectModelBaseAttribute
    {
        public string Name { get; private set; }

        public ObjectModelClassAttribute(string name)
        {
            Name = name;
        }

        public ObjectModelClassAttribute(string name, NotificationType context) : base(context)
        {
            Name = name;
        }
    }
}
