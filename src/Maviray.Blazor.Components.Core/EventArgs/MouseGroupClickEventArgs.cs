using Microsoft.AspNetCore.Components.Web;

namespace Maviray.Blazor.Components.Core.EventArgs;

public class MouseGroupClickEventArgs : MouseClickEventArgs
{
    public string? ButtonGroupId { get; init; }

    public MouseGroupClickEventArgs(string? buttonGroupId, string? buttonId) : base(buttonId)
    {
        ButtonGroupId = buttonGroupId;
    }

    public MouseGroupClickEventArgs(string? buttonGroupId, string? buttonId, MouseEventArgs args) : base(buttonId, args)
    {
        ButtonGroupId = buttonGroupId;
    }
}