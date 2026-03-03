using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class MaviTableCell
{
    public string? ColumnKey { get; set; }
    public int Id { get; set; }
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public string Value { get; set; } = string.Empty;
    public TableColumnDataType ColumnType { get; set; }
    public object? OriginalValue { get; set; } // typed value (parsed during data load)
}