using Maviray.Blazor.Components.Core.Attributes;
using Maviray.Blazor.Components.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Maviray.Blazor.Components.Core.Models.Tables;

public class TableDataCollectionFactory
{
    public static TableDataCollection FromDataSource<T>(IEnumerable<T> items) where T : ITableDataItem
    {
        var type = typeof(T);
        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttribute<TableColumnAttribute>() != null)
            .Select(property => new
            {
                Property = property,
                Display = property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? property.Name,
                Attribute = property.GetCustomAttribute<TableColumnAttribute>()!
            })
            .OrderBy(p => p.Attribute.Sequence)
            .ToList();

        var columns = properties.Select(property => new MaviTableColumn
        {
            Metadata = property.Attribute,
            Key = property.Property.Name,
            Title = property.Display,
            Sequence = property.Attribute.Sequence,
            Visible = true,
            DataType = property.Property.PropertyType,
            IsNavigational = property.Attribute.IsNavigational
        }).ToList();

        var rows = new List<MaviTableRow>();
        var idCounter = 1;

        foreach (var item in items)
        {
            var cells = new List<MaviTableCell>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var prop in properties)
            {
                var raw = prop.Property.GetValue(item);
                var valueStr = FormatValue(raw, prop.Attribute, prop.Property.PropertyType);

                cells.Add(new()
                {
                    ColumnKey = prop.Property.Name,
                    Value = valueStr,
                    ColumnType = MaviTableColumn.InferColumnType(prop.Property.PropertyType),
                    OriginalValue = raw
                });
            }

            rows.Add(new()
            {
                Id = idCounter++,
                Guid = Guid.NewGuid().ToString(),
                DataItemId = item.Id,
                DataItemGuid = item.Guid,
                Cells = cells,
                ContextActions = item.ContextMenu.ToList()
            });
        }

        return new()
        {
            Columns = columns,
            Rows = rows
        };
    }

    private static string FormatValue(object? raw, TableColumnAttribute attr, Type type)
    {
        if (raw == null)
        {
            return string.Empty;
        }

        if (type == typeof(bool))
        {
            return (bool)raw ? attr.BoolPositive : attr.BoolNegative;
        }

        if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            return ((DateTime)raw).ToString(attr.DatTimeFormat);
        }

        if (!type.IsEnum)
        {
            return raw.ToString() ?? string.Empty;
        }

        var method = typeof(EnumExtensions)
            .GetMethod("GetDisplay", BindingFlags.Public | BindingFlags.Static)?
            .MakeGenericMethod(type);

        if (method == null)
        {
            return raw.ToString() ?? string.Empty;
        }

        var result = method.Invoke(null, [raw]);
        return result?.ToString() ?? string.Empty;
    }

    public static string ExportToCsv(TableDataCollection grid)
    {
        ArgumentNullException.ThrowIfNull(grid.Columns, nameof(grid.Columns));

        var columns = grid.Columns
            .Where(c => c.Visible)
            .OrderBy(c => c.Sequence)
            .ToList();

        // Prepare CSV header
        var csv = new StringBuilder();
        csv.AppendLine(string.Join(",", columns.Select(c => CsvEscape(c.Title))));

        // Prepare CSV rows
        foreach (var row in grid.Rows)
        {
            var line = new List<string>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var col in columns)
            {
                // Find cell for this column
                var cell = row.Cells.FirstOrDefault(c => c.ColumnKey == col.Key);
                var val = cell?.Value ?? string.Empty;
                line.Add(CsvEscape(val));
            }

            csv.AppendLine(string.Join(",", line));
        }

        return csv.ToString();
    }

    // Escapes CSV value: quotes if needed, doubles internal quotes
    private static string CsvEscape(string value)
    {
        // ReSharper disable once InvertIf
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }

        return value;
    }
}