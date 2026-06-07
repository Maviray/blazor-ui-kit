using System.Globalization;
using System.Reflection;
using System.Resources;
using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TableColumnAttribute : Attribute
{
    private string? _boolNegative; // resource key, e.g. nameof(SampleResources.No)
    private string? _boolPositive; // resource key, e.g. nameof(SampleResources.Yes)

    public int Sequence { get; init; }
    public bool IsNavigational { get; init; }
    public string DatTimeFormat { get; init; } = "dd.MM.yyyy";

    public HorizontalPosition HorizontalTextAlignment { get; init; }

    public string BoolPositive
    {
        get
        {
            if (string.IsNullOrEmpty(_boolPositive))
            {
                _boolPositive = string.Empty;
            }

            return _boolPositive;
        }
    }

    public string BoolNegative
    {
        get
        {
            if (string.IsNullOrEmpty(_boolNegative))
            {
                _boolNegative = string.Empty;
            }

            return _boolNegative;
        }
    }

    public TableColumnAttribute(int sequence)
    {
        Sequence = sequence;
    }

    public TableColumnAttribute(int sequence, HorizontalPosition horizontalTextAlignment)
    {
        Sequence = sequence;
        HorizontalTextAlignment = horizontalTextAlignment;
    }

    public TableColumnAttribute(int sequence, bool isNavigational)
    {
        Sequence = sequence;
        IsNavigational = isNavigational;
    }

    public TableColumnAttribute(int sequence, bool isNavigational, HorizontalPosition horizontalTextAlignment)
    {
        Sequence = sequence;
        IsNavigational = isNavigational;
        HorizontalTextAlignment = horizontalTextAlignment;
    }

    public TableColumnAttribute(int sequence, string dateTimeFormat)
    {
        Sequence = sequence;
        DatTimeFormat = dateTimeFormat;
    }

    public TableColumnAttribute(int sequence, string dateTimeFormat, HorizontalPosition horizontalTextAlignment)
    {
        Sequence = sequence;
        DatTimeFormat = dateTimeFormat;
        HorizontalTextAlignment = horizontalTextAlignment;
    }

    public TableColumnAttribute(int sequence, string dateTimeFormat, bool isNavigational)
    {
        Sequence = sequence;
        IsNavigational = isNavigational;
        DatTimeFormat = dateTimeFormat;
    }

    public TableColumnAttribute(int sequence, string dateTimeFormat, bool isNavigational, HorizontalPosition horizontalTextAlignment)
    {
        Sequence = sequence;
        IsNavigational = isNavigational;
        DatTimeFormat = dateTimeFormat;
        HorizontalTextAlignment = horizontalTextAlignment;
    }

    public TableColumnAttribute(int sequence, string boolPositive, string boolNegative)
    {
        Sequence = sequence;
        _boolPositive = boolPositive;
        _boolNegative = boolNegative;
    }

    public TableColumnAttribute(int sequence, string boolPositive, string boolNegative, HorizontalPosition horizontalTextAlignment)
    {
        Sequence = sequence;
        _boolPositive = boolPositive;
        _boolNegative = boolNegative;
        HorizontalTextAlignment = horizontalTextAlignment;
    }

    public TableColumnAttribute(int sequence, bool isNavigational, string boolPositive, string boolNegative)
    {
        Sequence = sequence;
        IsNavigational = isNavigational;
        _boolPositive = boolPositive;
        _boolNegative = boolNegative;
    }

    public TableColumnAttribute(int sequence, bool isNavigational, string boolPositive, string boolNegative, HorizontalPosition horizontalTextAlignment)
    {
        Sequence = sequence;
        IsNavigational = isNavigational;
        _boolPositive = boolPositive;
        _boolNegative = boolNegative;
        HorizontalTextAlignment = horizontalTextAlignment;
    }
}