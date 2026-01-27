using Microsoft.AspNetCore.Components.Web;

namespace Maviray.Blazor.Components.Core.EventArgs;

public class MouseClickEventArgs(string? buttonId) : MouseEventArgs
{
    public string? ButtonId { get; init; } = buttonId;
}