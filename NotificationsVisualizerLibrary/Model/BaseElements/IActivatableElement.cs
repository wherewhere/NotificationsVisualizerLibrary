namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal interface IActivatableElement
    {
        string Arguments { get; set; }

        ActivationType ActivationType { get; set; }
    }
}
