using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.EventArgs;

public class DialogButtonClickEventArgs : ComponentEventArgs
{
    public DialogButtonClick ButtonClicked { get; init; }
    public MouseClickEventArgs MouseEventArgs { get; init; }

    public DialogButtonClickEventArgs(string? componentId, DialogButtonClick buttonClicked, MouseClickEventArgs mouseEventArgs) : base(componentId)
    {
        ButtonClicked = buttonClicked;
        MouseEventArgs = mouseEventArgs;
    }
}