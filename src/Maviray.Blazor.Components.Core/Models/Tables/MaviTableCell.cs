using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class MaviTableCell
{
    public string? ColumnKey { get; set; }
    public int Id { get; set; }
    // Created on first read: a large table builds one cell per row and column, and most never need an id.
    public string Guid
    {
        get => field ??= System.Guid.NewGuid().ToString();
        set => field = value;
    }
    public string Value { get; set; } = string.Empty;
    public TableColumnDataType ColumnType { get; set; }
    public object? OriginalValue { get; set; } // typed value (parsed during data load)
}