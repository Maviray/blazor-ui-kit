namespace Maviray.Blazor.Components.Core.Models.Tables;

public class TableParameters
{
    public string? ContentMenuTitle { get; set; }
    public string? NoDataAvailableTitle { get; set; }
    public string? RowsPerPageText { get; set; } = "Rows per page";
    public string? SelectNumberOfRowsToDisplayText { get; set; } = "Select number of rows to display";

    public string? GoToPreviousPageText { get; set; } = "Go to previous page";
    public string? GoToNextPageText { get; set; } = "Go to next page";
}