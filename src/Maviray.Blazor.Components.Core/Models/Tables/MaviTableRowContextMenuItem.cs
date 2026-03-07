using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class MaviTableRowContextMenuItem
{
    public int RowId { get; set; }
    public string? RowGuid { get; set; }
    public int Id { get; set; }
    public string? Guid { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool Disabled { get; set; }

    public ElementVariant ElementVariant { get; set; }
    public ThemeColorScheme ThemeColorScheme { get; set; }
    public ElementSize ElementSize { get; set; }
}