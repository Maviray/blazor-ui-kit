namespace Maviray.Blazor.Components.Core.EventArgs;

public class MouseGroupClickEventArgs(string? buttonGroupId, string? buttonId) : MouseClickEventArgs(buttonId)
{
    public string? ButtonGroupId { get; init; } = buttonGroupId;
}