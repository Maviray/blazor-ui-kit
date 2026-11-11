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

    /// <summary>
    ///     Finds the cell for a column. A plain loop: rows have few cells, and this runs for every cell
    ///     on every render, where a LINQ query would allocate.
    /// </summary>
    public MaviTableCell? GetCell(string columnKey)
    {
        foreach (var cell in Cells)
        {
            if (string.Equals(cell.ColumnKey, columnKey, StringComparison.Ordinal))
            {
                return cell;
            }
        }

        return null;
    }

    public object? GetCellValue(string columnKey) => GetCell(columnKey)?.OriginalValue;
}
