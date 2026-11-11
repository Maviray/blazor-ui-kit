using Maviray.Blazor.Components.Core.Options;
using Maviray.Blazor.Components.Material.Tests.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Material.Tests.Components;

/// <summary>
///     Component lifecycle traces are diagnostics (Debug), and nothing logs at Information on routine use.
/// </summary>
public class LoggingTests : ComponentTestBase
{
    private readonly CapturingLoggerProvider _logs = new();

    public LoggingTests()
    {
        // AddLogging() filters out Debug by default; let every level through so levels can be asserted.
        Services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace));
        Services.AddSingleton<ILoggerProvider>(_logs);
    }

    [Fact]
    public void LifecycleLogging_WritesDebugEntries()
    {
        Services.AddSingleton<IMaviComponentOptions>(new MaviComponentOptions { EnableLifecycleLogging = true });

        Render<MaviInputString>(p => p.Bind(x => x.Value, string.Empty, _ => { }));

        var libraryEntries = LibraryEntries();
        libraryEntries.Should().NotBeEmpty();
        libraryEntries.Should().OnlyContain(e => e.Level == LogLevel.Debug);
    }

    [Fact]
    public async Task AutoCompleteSearch_WritesNoInformationLogs()
    {
        string? value = null;
        var cut = Render<MaviAutoComplete<string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.OnSearch, _ => Task.FromResult<IEnumerable<string>>(["apple"]))
            .Add(x => x.DebounceDelay, 0));

        await cut.Find("input").InputAsync(new() { Value = "a" });

        LibraryEntries().Should().NotContain(e => e.Level >= LogLevel.Information);
    }

    private List<(string Category, LogLevel Level, string Message)> LibraryEntries() =>
        _logs.Entries.Where(e => e.Category.StartsWith("Maviray.", StringComparison.Ordinal)).ToList();
}
