using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;

namespace Maviray.Blazor.Components.Material.Components.Tabs;

public class MaviTabContext
{
    public string? NavBarId { get; set; }
    public string? SelectedTabId { get; set; }
    public ElementSize ElementSize { get; set; }
    public ThemeColorScheme ThemeColorScheme { get; set; }

    public event EventHandler<TabClickEventArgs>? OnTabClicked;

    public void OnTabClick(MouseClickEventArgs args)
    {
        SelectedTabId = args.ButtonId;
        OnTabClicked?.Invoke(this, new(NavBarId, args.ButtonId, args));
    }
}