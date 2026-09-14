using Bunit;
using Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Tests.Components.Editors.Wysiwyg;

public class WysiwygEditorTests : ComponentTestBase
{
    public WysiwygEditorTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_Toolbar_And_EditableSurface()
    {
        var cut = Render<WysiwygEditor>();

        cut.FindAll("[data-mavi-wysiwyg-toolbar]").Count.Should().Be(1);

        var surface = cut.Find("[data-mavi-wysiwyg-surface]");
        surface.GetAttribute("contenteditable").Should().Be("true");
        surface.GetAttribute("role").Should().Be("textbox");
    }

    [Fact]
    public async Task GetContentAsync_ReturnsHtml_FromModule()
    {
        JSInterop.Setup<string>("getHtml", _ => true).SetResult("<p>hello</p>");

        var cut = Render<WysiwygEditor>();
        var html = await cut.Instance.GetContentAsync();

        html.Should().Be("<p>hello</p>");
    }

    [Fact]
    public async Task SetContentAsync_CallsModule_And_RaisesContentChanged()
    {
        JSInterop.SetupVoid("setHtml", _ => true).SetVoidResult();
        string? changed = null;

        var cut = Render<WysiwygEditor>(p => p
            .Add(x => x.ContentChanged, EventCallback.Factory.Create<string?>(this, v => changed = v)));

        await cut.Instance.SetContentAsync("<p>abc</p>");

        changed.Should().Be("<p>abc</p>");
        JSInterop.VerifyInvoke("setHtml");
    }

    [Fact]
    public async Task Save_Invokes_OnSave_WithCurrentHtml()
    {
        JSInterop.Setup<string>("getHtml", _ => true).SetResult("<p>saved</p>");
        string? saved = null;

        var cut = Render<WysiwygEditor>(p => p
            .Add(x => x.ShowSaveButton, true)
            .Add(x => x.OnSave, EventCallback.Factory.Create<string>(this, v => saved = v)));

        await cut.Find("[data-mavi-wysiwyg-save]").ClickAsync(new());

        saved.Should().Be("<p>saved</p>");
    }

    [Fact]
    public async Task Refresh_WithProvider_LoadsReturnedHtml()
    {
        JSInterop.SetupVoid("setHtml", _ => true);

        var cut = Render<WysiwygEditor>(p => p
            .Add(x => x.ShowRefreshButton, true)
            .Add(x => x.RefreshContentProvider, () => Task.FromResult<string?>("<p>fresh</p>")));

        await cut.Find("[data-mavi-wysiwyg-refresh]").ClickAsync(new());

        JSInterop.VerifyInvoke("setHtml");
        var invocation = JSInterop.Invocations["setHtml"].Last();
        invocation.Arguments[1].Should().Be("<p>fresh</p>");
    }

    [Fact]
    public async Task Refresh_WithNullProvider_ClearsSurface()
    {
        JSInterop.SetupVoid("setHtml", _ => true);

        var cut = Render<WysiwygEditor>(p => p.Add(x => x.ShowRefreshButton, true));

        await cut.Find("[data-mavi-wysiwyg-refresh]").ClickAsync(new());

        var invocation = JSInterop.Invocations["setHtml"].Last();
        invocation.Arguments[1].Should().Be(string.Empty);
    }

    [Fact]
    public async Task BoldButton_Invokes_Exec_Bold()
    {
        JSInterop.Setup<string>("exec", _ => true).SetResult("<b>x</b>");

        var cut = Render<WysiwygEditor>();

        await cut.Find("[data-mavi-cmd='bold']").ClickAsync(new());

        var invocation = JSInterop.Invocations["exec"].Last();
        invocation.Arguments[1].Should().Be("bold");
    }

    [Fact]
    public void InlineGroup_Renders_AllFiveButtons()
    {
        var cut = Render<WysiwygEditor>();

        cut.FindAll("[data-mavi-cmd='bold']").Count.Should().Be(1);
        cut.FindAll("[data-mavi-cmd='italic']").Count.Should().Be(1);
        cut.FindAll("[data-mavi-cmd='underline']").Count.Should().Be(1);
        cut.FindAll("[data-mavi-cmd='strikeThrough']").Count.Should().Be(1);
        cut.FindAll("[data-mavi-cmd='removeFormat']").Count.Should().Be(1);
    }

    [Fact]
    public async Task StylePopover_Toggles_Open_And_Closed()
    {
        var cut = Render<WysiwygEditor>();

        cut.FindAll("[data-mavi-popover='style']").Count.Should().Be(0);

        await cut.Find("[data-mavi-popover-toggle='style']").ClickAsync(new());
        cut.FindAll("[data-mavi-popover='style']").Count.Should().Be(1);

        await cut.Find("[data-mavi-popover-toggle='style']").ClickAsync(new());
        cut.FindAll("[data-mavi-popover='style']").Count.Should().Be(0);
    }

    [Theory]
    [InlineData("insertUnorderedList")]
    [InlineData("insertOrderedList")]
    public async Task ListButtons_Invoke_Exec(string command)
    {
        JSInterop.Setup<string>("exec", _ => true).SetResult("<ul></ul>");
        var cut = Render<WysiwygEditor>();

        await cut.Find($"[data-mavi-cmd='{command}']").ClickAsync(new());

        JSInterop.Invocations["exec"].Last().Arguments[1].Should().Be(command);
    }

    [Fact]
    public async Task AlignCenter_Invokes_Exec_justifyCenter()
    {
        JSInterop.Setup<string>("exec", _ => true).SetResult("");
        var cut = Render<WysiwygEditor>();

        await cut.Find("[data-mavi-popover-toggle='paragraph']").ClickAsync(new());
        await cut.Find("[data-mavi-cmd='justifyCenter']").ClickAsync(new());

        JSInterop.Invocations["exec"].Last().Arguments[1].Should().Be("justifyCenter");
    }

    [Fact]
    public async Task FontFamily_Selection_Invokes_setFontName()
    {
        JSInterop.Setup<string>("setFontName", _ => true).SetResult("");
        var cut = Render<WysiwygEditor>();

        await cut.Find("[data-mavi-popover-toggle='font']").ClickAsync(new());
        await cut.Find("[data-mavi-font='Georgia']").ClickAsync(new());

        JSInterop.Invocations["setFontName"].Last().Arguments[1].Should().Be("Georgia");
    }

    [Fact]
    public async Task ForeColor_Selection_Invokes_setForeColor()
    {
        JSInterop.Setup<string>("setForeColor", _ => true).SetResult("");
        var cut = Render<WysiwygEditor>();

        await cut.Find("[data-mavi-popover-toggle='color']").ClickAsync(new());
        await cut.Find("[data-mavi-forecolor='#e03131']").ClickAsync(new());

        JSInterop.Invocations["setForeColor"].Last().Arguments[1].Should().Be("#e03131");
    }
}
