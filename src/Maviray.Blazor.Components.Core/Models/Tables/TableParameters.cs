using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class TableParameters
{
    public string? ContentMenuTitle { get; set; } = "Actions";
    public string? NoDataAvailableTitle { get; set; } = "No data available to display";
    public string? RowsPerPageText { get; set; } = "Rows per page";
    public string? SelectNumberOfRowsToDisplayText { get; set; } = "Select number of rows to display";
    public string? GoToPreviousPageText { get; set; } = "Go to previous page";
    public string? GoToNextPageText { get; set; } = "Go to next page";
    public string? FilterPlaceholderText { get; set; } = "Search...";
    public TableRowContextMenuDisplayStyle TableRowContextMenuDisplayStyle { get; set; }
    public bool EnableFilters { get; set; }
    public ElementHorizontalAlignment PaginationHorizontalAlignment { get; set; } = ElementHorizontalAlignment.Right;
}