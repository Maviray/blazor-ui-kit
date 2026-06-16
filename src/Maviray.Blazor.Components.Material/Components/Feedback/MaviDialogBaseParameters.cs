using Maviray.Blazor.Components.Core.Enums;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Components.Feedback;

public class MaviDialogBaseParameters
{
    public string? Title { get; set; }
    public ThemeColorScheme ThemeColorScheme { get; set; }
    public ZIndex ZIndex { get; set; } = ZIndex.Forty;
    public BackdropOpacity BackdropOpacity { get; set; } = BackdropOpacity.Darken;
    public ComponentRelativePosition ComponentRelativePosition { get; set; } = ComponentRelativePosition.Center;

    public bool CloseOnBackdropClick { get; set; }
    public bool CloseOnUserAction { get; set; } = true;

    public bool DisplayConfirmButton { get; set; } = true;
    public bool DisplayCancelButton { get; set; } = true;
    public bool DisplayCloseButton { get; set; } = true;

    public string? ConfirmButtonTitle { get; set; } = "Confirm";
    public string? CancelButtonTitle { get; set; } = "Cancel";
    public string? CloseButtonTitle { get; set; } = "Close";

    public string? Width { get; set; } = "w-full";
}