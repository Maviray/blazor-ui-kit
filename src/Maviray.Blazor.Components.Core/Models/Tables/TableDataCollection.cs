using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class TableDataCollection
{
    public int PageSize { get; set; } = 10;
    public int CurrentPage { get; set; } = 1;
    public int TotalRows => FilteredRows.Count;
    public int TotalPages => (TotalRows + PageSize - 1) / PageSize;

    public string GlobalSearchText { get; set; } = "";

    public int FirstRowIndex => Rows.Count == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;

    public int LastRowIndex
    {
        get
        {
            var lastIndex = CurrentPage * PageSize;
            return lastIndex > TotalRows ? TotalRows : lastIndex;
        }
    }

    public List<MaviTableColumn> Columns { get; set; } = [];
    public List<MaviTableRow> Rows { get; set; } = [];
    public Dictionary<string, string> SimpleColumnFilters { get; set; } = new(); // column key -> filter string

    public IEnumerable<KeyValuePair<string, string>> ColumnSelectOptions =>
        Columns.Select(column => new KeyValuePair<string, string>(column.Title, column.Title)).ToList();

    public string? SortColumnKey { get; set; }

    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

    public List<MaviTableRow> FilteredRows => FilterAndSort();

    private List<MaviTableRow> FilterAndSort()
    {
        IEnumerable<MaviTableRow> result = Rows;

        // Global search
        if (!string.IsNullOrWhiteSpace(GlobalSearchText))
        {
            result = result.Where(row =>
                row.Cells.Any(cell =>
                    !string.IsNullOrWhiteSpace(cell.Value) &&
                    cell.Value.Contains(GlobalSearchText, StringComparison.OrdinalIgnoreCase)));
        }

        // Simple column filters 
        foreach (var (columnKey, text) in SimpleColumnFilters)
        {
            result = result.Where(row =>
            {
                var cell = row.Cells.FirstOrDefault(c => c.ColumnKey == columnKey);
                return cell != null &&
                       !string.IsNullOrWhiteSpace(cell.Value) &&
                       cell.Value.Contains(text, StringComparison.OrdinalIgnoreCase);
            });
        }

        if (string.IsNullOrWhiteSpace(SortColumnKey))
        {
            return result.ToList();
        }

        // Sorting
        var comparer = new MaviTableSorter(SortColumnKey);

        result = SortOrder == SortOrder.Ascending
            ? result.OrderBy(r => r, comparer)
            : result.OrderByDescending(r => r, comparer);

        return result.ToList();
    }

    public IEnumerable<MaviTableRow> GetCurrentPageRows()
    {
        var filtered = GetFilteredRows();
        return filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);
    }

    public void ResetColumnVisibility()
    {
        foreach (var column in Columns)
        {
            column.Visible = true;
        }
    }

    public List<MaviTableRow> GetFilteredRows() => FilteredRows;

    public string GetColumnKey(string title) => Columns.FirstOrDefault(c => c.Title == title)?.Key ?? string.Empty;

    public MaviTableColumn? GetColumnByKey(string columnKey) => Columns.FirstOrDefault(c => c.Key == columnKey);
}