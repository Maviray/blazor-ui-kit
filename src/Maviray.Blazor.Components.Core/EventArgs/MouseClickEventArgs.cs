using Microsoft.AspNetCore.Components.Web;

namespace Maviray.Blazor.Components.Core.EventArgs;

public class MouseClickEventArgs : MouseEventArgs
{
    public string? ButtonId { get; init; }

    public MouseClickEventArgs(string? buttonId)
    {
        ButtonId = buttonId;
    }

    public MouseClickEventArgs(string? buttonId, MouseEventArgs mouseEventArgs) 
    {
        ButtonId = buttonId;
        AltKey = mouseEventArgs.AltKey;
        Button = mouseEventArgs.Button;
        Buttons = mouseEventArgs.Buttons;
        ClientX = mouseEventArgs.ClientX;
        ClientY = mouseEventArgs.ClientY;
        CtrlKey = mouseEventArgs.CtrlKey;
        Detail = mouseEventArgs.Detail;
        MetaKey = mouseEventArgs.MetaKey;
        OffsetX = mouseEventArgs.OffsetX;
        OffsetY = mouseEventArgs.OffsetY;
        PageX = mouseEventArgs.PageX;
        PageY = mouseEventArgs.PageY;
        ScreenX = mouseEventArgs.ScreenX;
        ScreenY = mouseEventArgs.ScreenY;
        ShiftKey = mouseEventArgs.ShiftKey;
        Type = mouseEventArgs.Type;
    }
}