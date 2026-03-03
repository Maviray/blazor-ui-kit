using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.EventArgs;

public class DialogButtonClickEventArgs : ComponentEventArgs
{
    public DialogButtonClickEventArgs(string? componentId, DialogButtonClick buttonClicked, MouseClickEventArgs mouseEventArgs) : base(componentId)
    {
        ButtonClicked = buttonClicked;
        MouseEventArgs = mouseEventArgs;
    }

    public DialogButtonClick ButtonClicked { get; init; }
    public MouseClickEventArgs MouseEventArgs { get; init; }
}