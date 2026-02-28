namespace Maviray.Blazor.Components.Core.EventArgs;

public class ComponentEventArgs : System.EventArgs
{
    public ComponentEventArgs(string? componentId) : base()
    {
        ComponentId = componentId;
    }
    public string? ComponentId { get; init; }
}