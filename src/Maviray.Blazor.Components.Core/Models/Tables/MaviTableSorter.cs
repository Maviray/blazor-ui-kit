using System.Globalization;
using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

/// <summary>
///     Orders table rows (or their cells) by one column. Missing cells and null values sort after values.
///     Numbers, dates, booleans and enums compare by value; text, and values of mixed types, compare as
///     culture-aware, case-insensitive text.
/// </summary>
public class MaviTableSorter : IComparer<MaviTableRow>, IComparer<MaviTableCell?>
{
    private readonly string _columnKey;

    public MaviTableSorter(string columnKey)
    {
        _columnKey = columnKey ?? string.Empty;
    }

    public int Compare(MaviTableRow? x, MaviTableRow? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x == null)
        {
            return 1; // null rows last
        }

        if (y == null)
        {
            return -1;
        }

        return Compare(x.GetCell(_columnKey), y.GetCell(_columnKey));
    }

    public int Compare(MaviTableCell? x, MaviTableCell? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        var xValue = x?.OriginalValue;
        var yValue = y?.OriginalValue;

        if (xValue is null)
        {
            return yValue is null ? 0 : 1;
        }

        if (yValue is null)
        {
            return -1;
        }

        var columnType = x!.ColumnType == y!.ColumnType ? x.ColumnType : TableColumnDataType.String;

        if (columnType is not (TableColumnDataType.String or TableColumnDataType.Other)
            && xValue is not string
            && xValue.GetType() == yValue.GetType()
            && xValue is IComparable comparable)
        {
            return comparable.CompareTo(yValue);
        }

        return string.Compare(x.Value, y.Value, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
    }
}
