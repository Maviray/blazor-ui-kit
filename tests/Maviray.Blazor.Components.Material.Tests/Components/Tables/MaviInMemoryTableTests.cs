using Maviray.Blazor.Components.Core.Models.Tables;
using Maviray.Blazor.Components.Material.Components.Tables;

namespace Maviray.Blazor.Components.Material.Tests.Components.Tables;

public class MaviInMemoryTableTests : ComponentTestBase
{
    public MaviInMemoryTableTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void NewParametersFromParent_AreRendered()
    {
        var cut = Render<MaviInMemoryTable>(p => p
            .Add(x => x.FetchData, LoadEmptyAsync)
            .Add(x => x.NoDataContent, "<p>Loading soon</p>"));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Loading soon"));

        cut.Render(p => p.Add(x => x.NoDataContent, "<p>Nothing here yet</p>"));

        cut.Markup.Should().Contain("Nothing here yet");
    }

    [Fact]
    public void RowContextMenu_RendersItemsOnlyWhenOpen()
    {
        var cut = Render<MaviInMemoryTable>(p => p
            .Add(x => x.FetchData, () => Task.FromResult(TableDataCollectionFactory.FromDataSource([new Item { Name = "Report" }]))));

        DeleteButtons(cut).Should().BeEmpty();

        cut.Find("tbody [data-context-menu-id] > button").Click();

        DeleteButtons(cut).Should().HaveCount(1);
    }

    private static IEnumerable<AngleSharp.Dom.IElement> DeleteButtons(IRenderedComponent<MaviInMemoryTable> cut) =>
        cut.FindAll("button").Where(b => b.TextContent.Trim() == "Delete");

    private static async Task<TableDataCollection> LoadEmptyAsync()
    {
        await Task.Yield();
        return new();
    }

    private sealed class Item : ITableDataItem
    {
        public int Id { get; init; }
        public string? Guid { get; init; }
        public IEnumerable<MaviTableRowContextMenuItem> ContextMenu => [new() { Key = "delete", Title = "Delete" }];

        [Maviray.Blazor.Components.Core.Attributes.TableColumn(1)]
        public string Name { get; init; } = string.Empty;
    }
}
