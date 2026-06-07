using Maviray.Blazor.Components.Core.Attributes;
using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class MaviTableColumn
{
    private Type? _dataType;
    public string Key { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public bool Visible { get; set; } = true;
    public bool IsNavigational { get; init; }

    public HorizontalPosition HorizontalTextAlignment { get; set; }

    public TableColumnAttribute? Metadata { get; set; }

    public Type? DataType
    {
        get => _dataType;
        set
        {
            _dataType = value;
            ColumnType = InferColumnType(_dataType);
        }
    }

    public TableColumnDataType ColumnType { get; private set; }

    public static TableColumnDataType InferColumnType(Type? type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType.IsEnum)
        {
            return TableColumnDataType.Enum;
        }

        if (underlyingType == typeof(string))
        {
            return TableColumnDataType.String;
        }

        if (underlyingType == typeof(int) || underlyingType == typeof(long))
        {
            return TableColumnDataType.Integer;
        }

        if (underlyingType == typeof(decimal))
        {
            return TableColumnDataType.Decimal;
        }

        if (underlyingType == typeof(double) || underlyingType == typeof(float))
        {
            return TableColumnDataType.Double;
        }

        if (underlyingType == typeof(bool))
        {
            return TableColumnDataType.Boolean;
        }

        if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTime?))
        {
            return TableColumnDataType.DateTime;
        }

        return TableColumnDataType.Other;
    }
}