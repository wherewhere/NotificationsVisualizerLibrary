namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelEnumAttribute : ObjectModelBaseAttribute
    {
        public string Name { get; set; }

        public ObjectModelEnumAttribute(string name) => Name = name;

        public ObjectModelEnumAttribute(string name, NotificationType context) : base(context) => Name = name;
    }
}
