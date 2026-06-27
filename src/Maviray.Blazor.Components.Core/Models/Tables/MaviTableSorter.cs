using System.Globalization;
using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class MaviTableSorter : IComparer<MaviTableRow>
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

        var cx = GetCell(x);
        var cy = GetCell(y);

        switch (cx)
        {
            // rows missing the column go last
            case null when cy == null:
                return 0;
            case null:
                return 1;
        }

        if (cy == null)
        {
            return -1;
        }

        var type = cx.ColumnType == cy.ColumnType ? cx.ColumnType : TableColumnDataType.String;

        return type switch
        {
            TableColumnDataType.Integer => NullableCompareInt(cx.OriginalValue, cy.OriginalValue),
            TableColumnDataType.Decimal => NullableCompareDecimal(cx.OriginalValue, cy.OriginalValue),
            TableColumnDataType.Double => NullableCompareDouble(cx.OriginalValue, cy.OriginalValue),
            TableColumnDataType.Boolean => NullableCompareBool(cx.OriginalValue, cy.OriginalValue),
            TableColumnDataType.DateTime => NullableCompareDateTime(cx.OriginalValue, cy.OriginalValue),
            TableColumnDataType.Enum => EnumCompare(cx.OriginalValue, cy.OriginalValue),
            _ => StringCompare(cx.Value, cy.Value)
        };
    }

    private static int StringCompare(string? a, string? b)
    {
        var aa = a ?? string.Empty;
        var bb = b ?? string.Empty;
        return string.Compare(aa, bb, CultureInfo.CurrentCulture,
            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
    }

    private static int EnumCompare(object? a, object? b)
    {
        var aHas = a != null;
        var bHas = b != null;

        switch (aHas)
        {
            case true when bHas:
            {
                // Same enum type? compare underlying ints
                if (a is Enum ae && b is Enum be && ae.GetType() == be.GetType())
                {
                    var ai = Convert.ToInt32(ae, CultureInfo.InvariantCulture);
                    var bi = Convert.ToInt32(be, CultureInfo.InvariantCulture);
                    return ai.CompareTo(bi);
                }

                // Try numeric, else string
                try
                {
                    var ai = Convert.ToInt32(a, CultureInfo.InvariantCulture);
                    var bi = Convert.ToInt32(b, CultureInfo.InvariantCulture);
                    return ai.CompareTo(bi);
                }
                catch
                {
                    return StringCompare(a?.ToString(), b?.ToString());
                }
            }
            case true:
                return -1; // non-null first
        }

        return bHas ? 1 : 0;
    }

    private static int NullableCompareInt(object? a, object? b)
    {
        var aHas = TryAsInt(a, out var av);
        var bHas = TryAsInt(b, out var bv);

        return aHas switch
        {
            true when bHas => av.CompareTo(bv),
            true => -1,
            _ => bHas ? 1 : 0
        };
    }

    private static int NullableCompareDecimal(object? a, object? b)
    {
        var aHas = TryAsDecimal(a, out var av);
        var bHas = TryAsDecimal(b, out var bv);

        return aHas switch
        {
            true when bHas => av.CompareTo(bv),
            true => -1,
            _ => bHas ? 1 : 0
        };
    }

    private static int NullableCompareDouble(object? a, object? b)
    {
        var aHas = TryAsDouble(a, out var av);
        var bHas = TryAsDouble(b, out var bv);

        return aHas switch
        {
            true when bHas => av.CompareTo(bv),
            true => -1,
            _ => bHas ? 1 : 0
        };
    }

    private static int NullableCompareBool(object? a, object? b)
    {
        var aHas = TryAsBool(a, out var av);
        var bHas = TryAsBool(b, out var bv);

        return aHas switch
        {
            true when bHas => av.CompareTo(bv),
            true => -1,
            _ => bHas ? 1 : 0
        };
    }

    private static int NullableCompareDateTime(object? a, object? b)
    {
        var aHas = TryAsDateTime(a, out var av);
        var bHas = TryAsDateTime(b, out var bv);

        return aHas switch
        {
            true when bHas => av.CompareTo(bv),
            true => -1,
            _ => bHas ? 1 : 0
        };
    }

    private static bool TryAsInt(object? o, out int parsedInteger)
    {
        switch (o)
        {
            case int iv:
                parsedInteger = iv;
                return true;
            case IConvertible:
                try
                {
                    parsedInteger = Convert.ToInt32(o, CultureInfo.InvariantCulture);
                    return true;
                }
                catch
                {
                    // ignored
                }

                break;
        }

        parsedInteger = 0;
        return false;
    }

    private static bool TryAsDecimal(object? o, out decimal parsedDecimal)
    {
        switch (o)
        {
            case decimal dv:
                parsedDecimal = dv;
                return true;
            case IConvertible:
                try
                {
                    parsedDecimal = Convert.ToDecimal(o, CultureInfo.InvariantCulture);
                    return true;
                }
                catch
                {
                    // ignored
                }

                break;
        }

        parsedDecimal = 0m;
        return false;
    }

    private static bool TryAsDouble(object? o, out double parsedDouble)
    {
        switch (o)
        {
            case double dv:
                parsedDouble = dv;
                return true;
            case IConvertible:
                try
                {
                    parsedDouble = Convert.ToDouble(o, CultureInfo.InvariantCulture);
                    return true;
                }
                catch
                {
                    // ignored
                }

                break;
        }

        parsedDouble = 0d;
        return false;
    }

    private static bool TryAsBool(object? o, out bool parsedBoolean)
    {
        switch (o)
        {
            case bool bv:
                parsedBoolean = bv;
                return true;
            case IConvertible:
                try
                {
                    parsedBoolean = Convert.ToBoolean(o, CultureInfo.InvariantCulture);
                    return true;
                }
                catch
                {
                    // ignored
                }

                break;
        }

        parsedBoolean = false;
        return false;
    }

    private static bool TryAsDateTime(object? o, out DateTime parsedDateTime)
    {
        switch (o)
        {
            case DateTime dt:
                parsedDateTime = dt;
                return true;
            // Adjust culture/format as needed
            case string s when DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDateTime):
                return true;
            case string s when DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsedDateTime):
                return true;
            default:
                parsedDateTime = default;
                return false;
        }
    }

    private MaviTableCell? GetCell(MaviTableRow? row)
    {
        return row?.Cells.FirstOrDefault(c => c.ColumnKey == _columnKey);
    }
}