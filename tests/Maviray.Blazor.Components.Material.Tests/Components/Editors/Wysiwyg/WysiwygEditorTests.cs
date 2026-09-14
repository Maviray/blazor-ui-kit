using Bunit;
using Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

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

    [Fact]
    public async Task LinkDialog_Insert_Invokes_insertLink()
    {
        JSInterop.Setup<string>("insertLink", _ => true).SetResult("");
        var cut = Render<WysiwygEditor>();

        await cut.Find("[data-mavi-popover-toggle='link']").ClickAsync(new());
        cut.Find("[data-mavi-link-url]").Input("https://example.com");
        cut.Find("[data-mavi-link-text]").Input("Example");
        await cut.Find("[data-mavi-link-insert]").ClickAsync(new());

        var inv = JSInterop.Invocations["insertLink"].Last();
        inv.Arguments[1].Should().Be("https://example.com");
        inv.Arguments[2].Should().Be("Example");
    }

    [Fact]
    public async Task PickingImage_Embeds_Base64_Via_insertImage()
    {
        JSInterop.Setup<string>("insertImage", _ => true).SetResult("");
        var cut = Render<WysiwygEditor>();

        var file = InputFileContent.CreateFromText("PNGDATA", "pic.png", contentType: "image/png");
        cut.FindComponent<InputFile>().UploadFiles(file);

        cut.WaitForAssertion(() => JSInterop.Invocations.Identifiers.Should().Contain("insertImage"));

        JSInterop.Invocations["insertImage"].Last().Arguments[1]!.ToString()
            .Should().Be("data:image/png;base64,UE5HREFUQQ==");
    }

    [Fact]
    public async Task TableGrid_Insert_Invokes_insertTable_WithDimensions()
    {
        JSInterop.Setup<string>("insertTable", _ => true).SetResult("");
        var cut = Render<WysiwygEditor>();

        await cut.Find("[data-mavi-popover-toggle='table']").ClickAsync(new());
        await cut.Find("[data-mavi-table-cell='3x4']").ClickAsync(new());

        var inv = JSInterop.Invocations["insertTable"].Last();
        inv.Arguments[1].Should().Be(3); // rows
        inv.Arguments[2].Should().Be(4); // cols
    }

    [Fact]
    public async Task CodeView_Toggle_Shows_RawHtml_Textarea()
    {
        JSInterop.Setup<string>("getHtml", _ => true).SetResult("<p>raw</p>");
        JSInterop.SetupVoid("setHtml", _ => true);
        var cut = Render<WysiwygEditor>();

        cut.FindAll("[data-mavi-wysiwyg-codearea]").Count.Should().Be(0);

        await cut.Find("[data-mavi-wysiwyg-codeview]").ClickAsync(new());
        var textarea = cut.Find("[data-mavi-wysiwyg-codearea]");
        textarea.GetAttribute("value").Should().Contain("<p>raw</p>");

        await cut.Find("[data-mavi-wysiwyg-codeview]").ClickAsync(new());
        JSInterop.VerifyInvoke("setHtml");
    }

    [Fact]
    public async Task Fullscreen_Toggle_Adds_FixedInset_Class()
    {
        JSInterop.SetupVoid("toggleFullscreen", _ => true);
        var cut = Render<WysiwygEditor>();

        await cut.Find("[data-mavi-wysiwyg-fullscreen]").ClickAsync(new());

        cut.Find("[data-mavi-wysiwyg-root]").GetAttribute("class").Should().Contain("fixed");
    }

    [Fact]
    public void ReadOnly_Hides_Toolbar_And_Locks_Surface()
    {
        var cut = Render<WysiwygEditor>(p => p.Add(x => x.ReadOnly, true));

        cut.Find("[data-mavi-wysiwyg-toolbar]").HasAttribute("hidden").Should().BeTrue();
        cut.Find("[data-mavi-wysiwyg-surface]").GetAttribute("contenteditable").Should().Be("false");
    }

    [Fact]
    public async Task Disabled_Prevents_Exec()
    {
        JSInterop.Setup<string>("exec", _ => true).SetResult("");
        var cut = Render<WysiwygEditor>(p => p.Add(x => x.Disabled, true));

        // When Disabled the toolbar still renders (only ReadOnly hides it), but every
        // command handler guards on IsInteractive, so clicking Bold must not call exec.
        cut.Find("[data-mavi-wysiwyg-surface]").GetAttribute("contenteditable").Should().Be("false");
        await cut.Find("[data-mavi-cmd='bold']").ClickAsync(new());
        JSInterop.Invocations.Identifiers.Should().NotContain("exec");
    }
}
