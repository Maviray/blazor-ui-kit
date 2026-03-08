namespace Maviray.Blazor.Components.Core.Models.Tables;

public class MaviTableRow
{
    public int Id { get; set; }
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public int DataItemId { get; set; }
    public string? DataItemGuid { get; set; }
    public bool Selected { get; set; }
    public bool ContextMenuVisible { get; set; }
    public string ContextMenuId => $"context-menu-{Guid}";
    public List<MaviTableCell> Cells { get; set; } = [];

    public bool HasContextActions => ContextActions.Count > 0;

    public List<MaviTableRowContextMenuItem> ContextActions { get; set; } = [];

    public object? GetCellValue(string columnKey)
        => Cells.FirstOrDefault(c => c.ColumnKey == columnKey)?.OriginalValue;
}