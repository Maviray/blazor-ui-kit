namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

public class MaviAutoCompleteTests : ComponentTestBase
{
    private readonly Dictionary<string, TaskCompletionSource<IEnumerable<string>>> _pendingSearches = new();
    private string? _value;

    public MaviAutoCompleteTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task SlowEarlierSearch_DoesNotOverwriteNewerResults()
    {
        var cut = RenderAutoComplete(ControlledSearch);

        var firstSearch = cut.Find("input").InputAsync(new() { Value = "a" });
        var secondSearch = cut.Find("input").InputAsync(new() { Value = "ab" });

        // The newer search answers first, then the older one arrives late.
        _pendingSearches["ab"].SetResult(["ab-result"]);
        await secondSearch;
        _pendingSearches["a"].SetResult(["a-result"]);
        await firstSearch;

        cut.WaitForAssertion(() =>
            cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).Should().Equal("ab-result"));
    }

    [Fact]
    public async Task OpeningResults_DoesNotEvaluateScript()
    {
        var cut = RenderAutoComplete(_ => Task.FromResult<IEnumerable<string>>(["apple"]));

        await cut.Find("input").InputAsync(new() { Value = "a" });

        cut.WaitForAssertion(() => cut.FindAll("[role=option]").Should().HaveCount(1));
        JSInterop.Invocations.Identifiers.Should().NotContain("eval");

        // Width now comes from CSS: the list spans its positioned container.
        cut.Find("[role=listbox]").ClassList.Should().Contain(["left-0", "right-0"]);
    }

    private Task<IEnumerable<string>> ControlledSearch(string query)
    {
        var search = new TaskCompletionSource<IEnumerable<string>>();
        _pendingSearches[query] = search;
        return search.Task;
    }

    private IRenderedComponent<MaviAutoComplete<string>> RenderAutoComplete(Func<string, Task<IEnumerable<string>>> search) =>
        Render<MaviAutoComplete<string>>(p => p
            .Bind(x => x.Value, _value, v => _value = v)
            .Add(x => x.OnSearch, search)
            .Add(x => x.DebounceDelay, 0));
}
