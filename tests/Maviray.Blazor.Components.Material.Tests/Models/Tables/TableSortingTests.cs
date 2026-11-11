using Maviray.Blazor.Components.Core.Attributes;
using Maviray.Blazor.Components.Core.Models.Tables;

namespace Maviray.Blazor.Components.Material.Tests.Models.Tables;

public class TableSortingTests
{
    [Fact]
    public void LongColumn_SortsByValue()
    {
        // Values beyond int range used to fail conversion and sort as if they were missing.
        var sorted = SortedValues([new() { Big = 5_000_000_000 }, new() { Big = 1 }, new() { Big = 3_000_000_000 }], nameof(Item.Big));

        sorted.Should().Equal(1L, 3_000_000_000L, 5_000_000_000L);
    }

    [Fact]
    public void DateColumn_SortsChronologically()
    {
        var sorted = SortedValues(
            [new() { When = new(2026, 3, 1) }, new() { When = new(2025, 1, 1) }, new() { When = new(2026, 1, 1) }],
            nameof(Item.When));

        sorted.Should().Equal(new DateTime(2025, 1, 1), new DateTime(2026, 1, 1), new DateTime(2026, 3, 1));
    }

    [Fact]
    public void EnumColumn_SortsByDeclarationOrder()
    {
        var sorted = SortedValues(
            [new() { Level = Priority.High }, new() { Level = Priority.Low }, new() { Level = Priority.Medium }],
            nameof(Item.Level));

        sorted.Should().Equal(Priority.Low, Priority.Medium, Priority.High);
    }

    [Fact]
    public void TextColumn_SortsIgnoringCase()
    {
        var sorted = SortedValues([new() { Name = "b" }, new() { Name = "A" }, new() { Name = "c" }], nameof(Item.Name));

        sorted.Should().Equal("A", "b", "c");
    }

    [Fact]
    public void MissingValues_SortLast()
    {
        var sorted = SortedValues([new() { Score = 2 }, new() { Score = null }, new() { Score = 1 }], nameof(Item.Score));

        sorted.Should().Equal(1, 2, null);
    }

    [Fact]
    public void Descending_ReversesOrder()
    {
        var sorted = SortedValues([new() { Score = 1 }, new() { Score = 3 }, new() { Score = 2 }], nameof(Item.Score), descending: true);

        sorted.Should().Equal(3, 2, 1);
    }

    private static IEnumerable<object?> SortedValues(Item[] items, string column, bool descending = false)
    {
        var collection = TableDataCollectionFactory.FromDataSource(items);

        collection.SortByColumn(column);

        if (descending)
        {
            // A second sort on the same column flips the order.
            collection.SortByColumn(column);
        }

        return collection.FilteredRows.Select(r => r.Cells.Single(c => c.ColumnKey == column).OriginalValue).ToList();
    }

    private enum Priority
    {
        Low,
        Medium,
        High
    }

    private sealed class Item : ITableDataItem
    {
        public int Id { get; init; }
        public string? Guid { get; init; }
        public IEnumerable<MaviTableRowContextMenuItem> ContextMenu => [];

        [TableColumn(1)]
        public long Big { get; init; }

        [TableColumn(2)]
        public DateTime When { get; init; }

        [TableColumn(3)]
        public Priority Level { get; init; }

        [TableColumn(4)]
        public string Name { get; init; } = string.Empty;

        [TableColumn(5)]
        public int? Score { get; init; }
    }
}
