namespace Maviray.Blazor.Components.Core.Models.Tables;

public interface ITableDataItem
{
    public int Id { get; }
    public string? Guid { get; }
    IEnumerable<MaviTableRowContextMenuItem> ContextMenu { get; }
}