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
}
