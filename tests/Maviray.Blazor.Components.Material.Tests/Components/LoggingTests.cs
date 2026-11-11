using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Extensions;
using Maviray.Blazor.Components.Core.Options;
using Maviray.Blazor.Components.Core.Services;
using Maviray.Blazor.Components.Material.Components;
using Maviray.Blazor.Components.Material.Components.DataDisplay;
using Maviray.Blazor.Components.Material.Components.DataDisplay.Icons;
using Maviray.Blazor.Components.Material.Components.DataDisplay.MaviToastr;
using Maviray.Blazor.Components.Material.Components.Feedback;
using Maviray.Blazor.Components.Material.Components.Menus;
using Maviray.Blazor.Components.Material.Components.Tables;
using Maviray.Blazor.Components.Material.Components.Tabs;
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
        JSInterop.Mode = JSRuntimeMode.Loose;
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

    /// <summary>One component per base class and per component that writes lifecycle traces itself.</summary>
    public static TheoryData<string> TracingComponents =>
    [
        "InputStringNullable", "InputInteger", "InputLong", "InputShort", "TextArea", "InputDate", "InputDateNullable",
        "Checkbox", "Switch", "Toggle", "RadioButton", "Dropdown", "MultiSelect", "AutoComplete", "Button",
        "ButtonGroup", "IconSolid", "IconMini", "IconOnline", "Backdrop", "TabNavBar", "Toastr", "DropDownMenu",
        "MenuVertical", "InMemoryTable"
    ];

    [Theory]
    [MemberData(nameof(TracingComponents))]
    public void LifecycleLogging_ComponentWritesOnlyDebugEntries(string component)
    {
        Services.AddSingleton<IMaviComponentOptions>(new MaviComponentOptions { EnableLifecycleLogging = true });

        RenderTracingComponent(component);

        var libraryEntries = LibraryEntries();
        libraryEntries.Should().NotBeEmpty("{0} writes lifecycle traces when lifecycle logging is on", component);
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

    [Fact]
    public void LifecycleTrace_DebugDisabled_DoesNotCallTheLogger()
    {
        var logger = new RecordingLogger(LogLevel.Information);

        logger.LogDebugLifeCycle("field-1", typeof(MaviInputString));
        logger.LogDebugLifeCycle(typeof(MaviInputString), "field-1", "opened");

        logger.Entries.Should().BeEmpty();
    }

    [Fact]
    public void LifecycleTrace_DebugEnabled_WritesOneDebugEntryPerCall()
    {
        var logger = new RecordingLogger(LogLevel.Debug);

        logger.LogDebugLifeCycle("field-1", typeof(MaviInputString));
        logger.LogDebugLifeCycle(typeof(MaviInputString), "field-1", "opened");

        logger.Entries.Should().HaveCount(2).And.OnlyContain(e => e.Level == LogLevel.Debug);
        logger.Entries[0].Message.Should().StartWith($"[MaviInputString.{nameof(LifecycleTrace_DebugEnabled_WritesOneDebugEntryPerCall)}]")
            .And.EndWith("-[field-1]");
        logger.Entries[1].Message.Should().EndWith("-[field-1]: opened");
    }

    private static AngleSharp.Dom.IElement ClickHandlerOf<TComponent>(IRenderedComponent<TComponent> cut) where TComponent : IComponent =>
        cut.FindAll("*").First(element => element.HasAttribute("blazor:onclick"));

    private void RenderTracingComponent(string component)
    {
        // MaviMenuVertical injects it; apps register it themselves.
        Services.AddScoped<ICircuitStateService, CircuitStateService>();

        switch (component)
        {
            case "InputStringNullable":
                Render<MaviInputStringNullable>(p => p.Bind(x => x.Value, null, _ => { }));
                break;
            case "InputInteger":
                Render<MaviInputInteger>(p => p.Bind(x => x.Value, 0, _ => { }));
                break;
            case "InputLong":
                Render<MaviInputLong>(p => p.Bind(x => x.Value, 0L, _ => { }));
                break;
            case "InputShort":
                Render<MaviInputShort>(p => p.Bind(x => x.Value, (short)0, _ => { }));
                break;
            case "TextArea":
                Render<MaviTextArea>(p => p.Bind(x => x.Value, string.Empty, _ => { }));
                break;
            case "InputDate":
                Render<MaviInputDate>(p => p.Bind(x => x.Value, new DateTime(2026, 3, 9), _ => { }));
                break;
            case "InputDateNullable":
                Render<MaviInputDateNullable>(p => p.Bind(x => x.Value, null, _ => { }));
                break;
            // The boolean inputs trace when their value changes, not on render.
            case "Checkbox":
                Render<MaviCheckbox>(p => p.Bind(x => x.Value, false, _ => { })).Find("input").Change(true);
                break;
            case "Switch":
                ClickHandlerOf(Render<MaviSwitch>(p => p.Bind(x => x.Value, false, _ => { }))).Click();
                break;
            case "Toggle":
                ClickHandlerOf(Render<MaviToggle>(p => p.Bind(x => x.Value, false, _ => { }))).Click();
                break;
            case "RadioButton":
                Render<MaviRadioButton>(p => p.Bind(x => x.Value, false, _ => { })).Find("input").Change(true);
                break;
            case "Dropdown":
                Render<MaviDropdown<string, string>>(p => p
                    .Bind(x => x.Value, null, _ => { })
                    .Add(x => x.Items, ["apple"]));
                break;
            case "MultiSelect":
                Render<MaviMultiSelect<string, string>>(p => p
                    .Bind(x => x.Value, null, _ => { })
                    .Add(x => x.Items, ["apple"]));
                break;
            case "AutoComplete":
                Render<MaviAutoComplete<string>>(p => p
                    .Bind(x => x.Value, null, _ => { })
                    .Add(x => x.OnSearch, _ => Task.FromResult<IEnumerable<string>>(["apple"])));
                break;
            case "Button":
                Render<MaviButton>();
                break;
            case "ButtonGroup":
                Render<MaviButtonGroup>();
                break;
            case "IconSolid":
                Render<MaviIconSolid>(p => p.Add(x => x.IconSolidType, default(IconSolidType)));
                break;
            case "IconMini":
                Render<MaviIconMini>(p => p.Add(x => x.IconMiniType, default(IconMiniType)));
                break;
            case "IconOnline":
                Render<MaviIconOnline>(p => p.Add(x => x.IconOnlineType, default(IconOnlineType)));
                break;
            case "Backdrop":
                Render<MaviBackdrop>();
                break;
            case "TabNavBar":
                Render<MaviTabNavBar>(p => p.Add(x => x.ChildContent, SingleTab));
                break;
            case "Toastr":
                Render<MaviToastr>(p => p.Add(x => x.Notifications, Array.Empty<MaviToastNotification>()));
                break;
            case "DropDownMenu":
                Render<MaviDropDownMenu>(p => p.Add(x => x.MenuItems, Array.Empty<MenuItem>()));
                break;
            case "MenuVertical":
                Render<MaviMenuVertical>();
                break;
            case "InMemoryTable":
                Render<MaviInMemoryTable>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(component), component, "No render setup for this component.");
        }
    }

    private static RenderFragment SingleTab => builder =>
    {
        builder.OpenComponent<MaviTab>(0);
        builder.AddAttribute(1, nameof(MaviTab.Id), "tab-a");
        builder.AddAttribute(2, nameof(MaviTab.Title), "A");
        builder.CloseComponent();
    };

    private List<(string Category, LogLevel Level, string Message)> LibraryEntries() =>
        _logs.Entries.Where(e => e.Category.StartsWith("Maviray.", StringComparison.Ordinal)).ToList();

    /// <summary>A logger enabled from a minimum level that records each call it receives.</summary>
    private sealed class RecordingLogger(LogLevel minimumLevel) : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= minimumLevel;

        // Records even when the level is disabled: only the caller's guard keeps disabled traces out.
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception)));
    }
}
