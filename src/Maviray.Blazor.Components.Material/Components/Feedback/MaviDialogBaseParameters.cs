using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Material.Components.Feedback;

public class MaviDialogBaseParameters
{
    public string? Title { get; set; }
    public ThemeColorScheme ThemeColorScheme { get; set; }
    public ZIndex ZIndex { get; set; } = ZIndex.Forty;
    public BackdropOpacity BackdropOpacity { get; set; } = BackdropOpacity.Darken;
    public ComponentRelativePosition ComponentRelativePosition { get; set; } = ComponentRelativePosition.Center;
    public ElementSize SpinnerSize { get; set; }

    public bool CloseOnBackdropClick { get; set; }
    public bool CloseOnUserAction { get; set; } = true;

    public bool DisplayConfirmButton { get; set; } = true;
    public bool DisplayCancelButton { get; set; } = true;
    public bool DisplayCloseButton { get; set; } = true;

    public string? ConfirmButtonTitle { get; set; } = "Confirm";
    public string? CancelButtonTitle { get; set; } = "Cancel";
    public string? CloseButtonTitle { get; set; } = "Close";

    public string? Width { get; set; } = "w-full";

    public string? BackgroundColor { get; set; } = "bg-white";

    public string? DialogBoxCss { get; set; }

    public string? ContainerOverrideCss { get; set; }

    public void Update(MaviDialogBaseParameters parameters)
    {
        Title = parameters.Title;
        ThemeColorScheme = parameters.ThemeColorScheme;
        ZIndex = parameters.ZIndex;
        BackdropOpacity = parameters.BackdropOpacity;
        ComponentRelativePosition = parameters.ComponentRelativePosition;

        CloseOnBackdropClick = parameters.CloseOnBackdropClick;
        CloseOnUserAction = parameters.CloseOnUserAction;

        DisplayConfirmButton = parameters.DisplayConfirmButton;
        DisplayCancelButton = parameters.DisplayCancelButton;
        DisplayCloseButton = parameters.DisplayCloseButton;

        ConfirmButtonTitle = parameters.ConfirmButtonTitle;
        CancelButtonTitle = parameters.CancelButtonTitle;
        CloseButtonTitle = parameters.CloseButtonTitle;

        Width = parameters.Width;
        BackgroundColor = parameters.BackgroundColor;
        SpinnerSize = parameters.SpinnerSize;
        DialogBoxCss = parameters.DialogBoxCss;
    }

    public MaviDialogBaseParameters SetTitle(string? title)
    {
        Title = title;
        return this;
    }

    public MaviDialogBaseParameters SetModalCss(string? modalCss)
    {
        DialogBoxCss = modalCss;
        return this;
    }

    public MaviDialogBaseParameters SetWidth(string? width)
    {
        Width = width;
        return this;
    }

    public MaviDialogBaseParameters SetBackgroundColor(string? backgroundColor)
    {
        BackgroundColor = backgroundColor;
        return this;
    }
}