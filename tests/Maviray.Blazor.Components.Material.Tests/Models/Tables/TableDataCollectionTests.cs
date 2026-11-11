using System.ComponentModel.DataAnnotations;
using Maviray.Blazor.Components.Core.Attributes;
using Maviray.Blazor.Components.Core.Models.Tables;

namespace Maviray.Blazor.Components.Material.Tests.Models.Tables;

public class TableDataCollectionTests
{
    [Fact]
    public void EnumCells_ShowDisplayName()
    {
        var collection = TableDataCollectionFactory.FromDataSource(
        [
            new WorkItem { Name = "a", Priority = Priority.VeryHigh, OptionalPriority = Priority.VeryHigh }
        ]);

        CellValue(collection.Rows[0], nameof(WorkItem.Priority)).Should().Be("Very high");
        CellValue(collection.Rows[0], nameof(WorkItem.OptionalPriority)).Should().Be("Very high");
    }

    [Fact]
    public void EnumCells_WithoutDisplayName_ShowMemberName()
    {
        var collection = TableDataCollectionFactory.FromDataSource([new WorkItem { Name = "a", Priority = Priority.Low }]);

        CellValue(collection.Rows[0], nameof(WorkItem.Priority)).Should().Be("Low");
    }

    [Fact]
    public void ChangingGlobalSearch_RefiltersRows()
    {
        var collection = FromNames("apple", "banana", "cherry");
        collection.FilteredRows.Should().HaveCount(3);

        collection.GlobalSearchText = "an";

        collection.FilteredRows.Select(r => CellValue(r, nameof(WorkItem.Name))).Should().Equal("banana");
    }

    [Fact]
    public void ColumnFilter_ReturnsToFirstPage()
    {
        var collection = FromNames(Enumerable.Range(1, 25).Select(i => $"item-{i:00}").ToArray());
        collection.CurrentPage = 3;

        // Matches item-01 to item-09: a single page.
        collection.SetColumnFilter(nameof(WorkItem.Name), "item-0");

        collection.CurrentPage.Should().Be(1);
        collection.GetCurrentPageRows().Should().HaveCount(9);
    }

    [Fact]
    public void GlobalSearch_ReturnsToFirstPage()
    {
        var collection = FromNames(Enumerable.Range(1, 25).Select(i => $"item-{i:00}").ToArray());
        collection.CurrentPage = 3;

        collection.GlobalSearchText = "item-0";

        collection.CurrentPage.Should().Be(1);
        collection.GetCurrentPageRows().Should().HaveCount(9);
    }

    private static TableDataCollection FromNames(params string[] names) =>
        TableDataCollectionFactory.FromDataSource(names.Select(n => new WorkItem { Name = n }));

    private static string CellValue(MaviTableRow row, string columnKey) =>
        row.Cells.Single(c => c.ColumnKey == columnKey).Value;

    private enum Priority
    {
        Low,

        [Display(Name = "Very high")]
        VeryHigh
    }

    private sealed class WorkItem : ITableDataItem
    {
        public int Id { get; init; }
        public string? Guid { get; init; }
        public IEnumerable<MaviTableRowContextMenuItem> ContextMenu => [];

        [TableColumn(1)]
        public string Name { get; init; } = string.Empty;

        [TableColumn(2)]
        public Priority Priority { get; init; }

        [TableColumn(3)]
        public Priority? OptionalPriority { get; init; }
    }
}
