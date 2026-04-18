using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class TableDataCollection
{
    private readonly Dictionary<string, string> _columnFilters = new(); // column key -> filter string
    private List<MaviTableRow>? _cachedFilteredRows;
    private bool _filterCacheDirty = true;

    private IEnumerable<MaviTableRow>? _cachedPageRows;
    private int _lastPageNumber;

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
    public List<MaviTableRow> FilteredRows
    {
        get
        {
            if (!_filterCacheDirty && _cachedFilteredRows != null)
            {
                return _cachedFilteredRows;
            }

            _cachedFilteredRows = FilterAndSort();
            _filterCacheDirty = false;
            return _cachedFilteredRows;
        }
    }

    // Invalidate cache when filters/sorting changes
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (_pageSize == value)
            {
                return;
            }

            _pageSize = value;
            _filterCacheDirty = true;
        }
    }

    public IEnumerable<KeyValuePair<string, string>> ColumnSelectOptions =>
        Columns.Select(column => new KeyValuePair<string, string>(column.Title, column.Title)).ToList();

    public string? SortColumnKey { get; set; }

    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

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
        foreach (var (columnKey, text) in _columnFilters)
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
    
    public void ResetColumnVisibility()
    {
        foreach (var column in Columns)
        {
            column.Visible = true;
        }
    }

    public IEnumerable<MaviTableRow> GetCurrentPageRows()
    {
        if (_cachedPageRows != null && _lastPageNumber == CurrentPage && !_filterCacheDirty)
        {
            return _cachedPageRows;
        }

        _cachedPageRows = FilteredRows
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList(); // Materialize to prevent re-enumeration
        _lastPageNumber = CurrentPage;
        return _cachedPageRows;
    }

    public string GetColumnKey(string title) => Columns.FirstOrDefault(c => c.Title == title)?.Key ?? string.Empty;

    public MaviTableColumn? GetColumnByKey(string columnKey) => Columns.FirstOrDefault(c => c.Key == columnKey);

    public void SetColumnFilter(string columnKey, string filterText)
    {
        if (string.IsNullOrWhiteSpace(filterText))
        {
            _columnFilters.Remove(columnKey);
        }
        else
        {
            _columnFilters[columnKey] = filterText;
        }

        _filterCacheDirty = true;
    }

    public void SortByColumn(string columnKey)
    {
        if (SortColumnKey == columnKey)
        {
            SortOrder = SortOrder == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending;
        }
        else
        {
            SortColumnKey = columnKey;
            SortOrder = SortOrder.Ascending;
        }

        _filterCacheDirty = true;
    }
}