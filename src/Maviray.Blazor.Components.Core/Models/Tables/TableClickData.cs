namespace Maviray.Blazor.Components.Core.Models.Tables;

public record TableClickData(MaviTableRow? TableRow, MaviTableColumn? TableColumn)
{
    public MaviTableRow? Row => TableRow;
    public MaviTableColumn? Column => TableColumn;
}