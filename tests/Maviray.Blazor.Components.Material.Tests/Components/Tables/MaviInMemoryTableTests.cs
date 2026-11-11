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

    private static async Task<TableDataCollection> LoadEmptyAsync()
    {
        await Task.Yield();
        return new();
    }
}
