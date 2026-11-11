using Maviray.Blazor.Components.Core.Models.Tables;
using Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;
using Maviray.Blazor.Components.Material.Components.Tables;

namespace Maviray.Blazor.Components.Material.Tests.Components;

/// <summary>
///     Popups listen for clicks outside themselves only while they are open. A closed popup must
///     not register a document listener, because every page click would otherwise call into .NET
///     (one server round-trip per closed popup on Blazor Server).
/// </summary>
public class OutsideClickListenerTests : ComponentTestBase
{
    private const string DROPDOWN_MODULE = "./_content/Maviray.Blazor.Components.Material/js/maviDropDownJsInterop.js";
    private const string DATE_PICKER_MODULE = "./_content/Maviray.Blazor.Components.Material/js/maviDatePickerJsInterop.js";
    private const string REGISTER_GLOBAL = "registerGlobalOutsideClickListener";
    private const string UNREGISTER_GLOBAL = "unregisterGlobalOutsideClickListener";

    public OutsideClickListenerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task Dropdown_ListensOnlyWhileOpen()
    {
        var module = JSInterop.SetupModule(DROPDOWN_MODULE);
        string? value = null;
        var cut = Render<MaviDropdown<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple"]));

        module.Invocations["initializeDropdown"].Should().BeEmpty();

        cut.Find("input").Click();
        cut.WaitForAssertion(() => module.Invocations["initializeDropdown"].Should().HaveCount(1));

        await cut.InvokeAsync(cut.Instance.CloseFromJs);
        cut.WaitForAssertion(() => module.Invocations["disposeDropdown"].Should().HaveCount(1));
    }

    [Fact]
    public async Task MultiSelect_ListensOnlyWhileOpen()
    {
        var module = JSInterop.SetupModule(DROPDOWN_MODULE);
        List<string>? value = [];
        var cut = Render<MaviMultiSelect<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple"]));

        module.Invocations["initializeDropdown"].Should().BeEmpty();

        cut.Find("[role=combobox]").Click();
        cut.WaitForAssertion(() => module.Invocations["initializeDropdown"].Should().HaveCount(1));

        await cut.InvokeAsync(cut.Instance.CloseFromJs);
        cut.WaitForAssertion(() => module.Invocations["disposeDropdown"].Should().HaveCount(1));
    }

    [Fact]
    public async Task DateInput_ListensOnlyWhileCalendarIsOpen()
    {
        var module = JSInterop.SetupModule(DATE_PICKER_MODULE);
        var value = new DateTime(2026, 3, 9);
        var cut = Render<MaviInputDate>(p => p.Bind(x => x.Value, value, v => value = v));

        module.Invocations["initializeDatePicker"].Should().BeEmpty();

        cut.Find("input").Click();
        cut.WaitForAssertion(() => module.Invocations["initializeDatePicker"].Should().HaveCount(1));

        await cut.InvokeAsync(cut.Instance.CloseCalendar);
        cut.WaitForAssertion(() => module.Invocations["disposeDatePicker"].Should().HaveCount(1));
    }

    [Fact]
    public async Task Table_ListensOnlyWhileAContextMenuIsOpen()
    {
        var cut = Render<MaviInMemoryTable>(p => p
            .Add(x => x.ConstructContextMenu, () => Task.FromResult<IEnumerable<MaviTableContextMenuItem>>(
                [new MaviTableContextMenuItem { Id = "export", Title = "Export" }])));

        JSInterop.Invocations[REGISTER_GLOBAL].Should().BeEmpty();

        cut.Find($"[data-context-menu-id='{cut.Instance.ContextMenuId}'] button").Click();
        cut.WaitForAssertion(() => JSInterop.Invocations[REGISTER_GLOBAL].Should().HaveCount(1));

        await cut.InvokeAsync(() => cut.Instance.HandleOutsideClick(string.Empty));
        cut.WaitForAssertion(() => JSInterop.Invocations[UNREGISTER_GLOBAL].Should().HaveCount(1));
    }

    [Fact]
    public async Task Editor_ListensOnlyWhileAPopoverIsOpen()
    {
        var cut = Render<WysiwygEditor>();

        JSInterop.Invocations[REGISTER_GLOBAL].Should().BeEmpty();

        await cut.Find("[data-mavi-popover-toggle='style']").ClickAsync(new());
        cut.WaitForAssertion(() => JSInterop.Invocations[REGISTER_GLOBAL].Should().HaveCount(1));

        await cut.Find("[data-mavi-popover-toggle='style']").ClickAsync(new());
        cut.WaitForAssertion(() => JSInterop.Invocations[UNREGISTER_GLOBAL].Should().HaveCount(1));
    }
}
