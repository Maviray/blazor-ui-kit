using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Material.Components.Menus;

public interface IMenuItem
{
    public string? Guid { get; }
    public string? Key { get; set; }
    public string? Title { get; set; }
    public string? Icon { get; set; }
    public bool Disabled { get; set; }
    public bool Selected { get; set; }
    public string? BadgeText { get; set; }
    public ThemeColorScheme BadgeColor { get; set; }

    void MarkActive(string? guid);
}