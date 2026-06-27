namespace Maviray.Blazor.Components.Core.EventArgs;

public class ComponentEventArgs : System.EventArgs
{
    public string? ComponentId { get; init; }

    public ComponentEventArgs(string? componentId)
    {
        ComponentId = componentId;
    }
}