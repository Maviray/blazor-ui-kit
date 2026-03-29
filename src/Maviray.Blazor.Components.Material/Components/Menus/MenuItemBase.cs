using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Material.Components.Menus;

public class MenuItemBase : IMenuItem
{
    public string? Guid { get; protected set; } = System.Guid.NewGuid().ToString();
    public string? Key { get; set; }
    public string? Title { get; set; }
    public string? Icon { get; set; }
    public bool Disabled { get; set; }
    public bool Hidden { get; set; }
    public bool Selected { get; set; }
    public string? BadgeText { get; set; }
    public ThemeColorScheme BadgeColor { get; set; }

    public virtual void MarkActive(string? guid)
    {
        Selected = Guid == guid;
    }
}