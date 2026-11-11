using AngleSharp.Html.Parser;
using Ganss.Xss;
using Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;
using Microsoft.Extensions.DependencyInjection;

namespace Maviray.Blazor.Components.Material.Tests.Components.Editors.Wysiwyg;

/// <summary>
///     The editor cleans HTML both on the way into the page (so stored content from another user cannot
///     run script in this browser) and on the way out (so the app never receives unsafe HTML to store).
/// </summary>
public class WysiwygSanitizationTests : ComponentTestBase
{
    // Markup the editor's own toolbar produces; none of it may be lost to sanitization.
    private const string EDITOR_OUTPUT =
        "<h2>Title</h2>" +
        "<p><b>b</b> <i>i</i> <u>u</u> <strike>s</strike></p>" +
        "<ul><li>a</li></ul><ol><li>b</li></ol>" +
        "<blockquote>q</blockquote><pre>code</pre>" +
        "<p><font face=\"Georgia\" color=\"#e03131\">font</font> <font style=\"font-size: 24px;\">size</font>" +
        " <span style=\"background-color: rgb(255, 212, 59);\">mark</span></p>" +
        "<div style=\"text-align: center;\">centered</div>" +
        "<p><a href=\"https://example.com\" target=\"_blank\" rel=\"noopener\">web</a>" +
        " <a href=\"mailto:someone@example.com\">mail</a> <a href=\"/docs/page\">relative</a></p>" +
        "<p><img src=\"data:image/png;base64,UE5HREFUQQ==\"></p>" +
        "<table style=\"border-collapse:collapse;width:100%\"><tr>" +
        "<td style=\"border:1px solid #ccc;padding:6px;min-width:32px\">cell</td></tr></table>";

    private string? _changed;

    public WysiwygSanitizationTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void InitialContent_IsSanitizedBeforeReachingThePage()
    {
        var cut = Render<WysiwygEditor>(p => p
            .Add(x => x.Content, "<p>Hi</p><img src=\"x.png\" onerror=\"alert(1)\"><script>alert(2)</script>"));

        cut.WaitForAssertion(() => JSInterop.Invocations["setHtml"].Should().HaveCount(1));
        var html = (string)JSInterop.Invocations["setHtml"].Single().Arguments[1]!;

        html.Should().Contain("<p>Hi</p>").And.Contain("x.png");
        html.Should().NotContain("onerror").And.NotContain("<script");
    }

    [Fact]
    public async Task SetContent_SanitizesForThePageAndTheApp()
    {
        var cut = RenderTrackingChanges();

        await cut.Instance.SetContentAsync("<a href=\"javascript:alert(1)\">x</a><iframe src=\"https://evil.example\"></iframe>");

        var html = (string)JSInterop.Invocations["setHtml"].Last().Arguments[1]!;
        html.Should().NotContain("javascript:").And.NotContain("iframe");
        _changed.Should().NotContain("javascript:").And.NotContain("iframe");
    }

    [Fact]
    public async Task EditedContent_IsSanitizedBeforeTheAppReceivesIt()
    {
        var cut = RenderTrackingChanges();

        await cut.InvokeAsync(() => cut.Instance.OnEditorBlurAsync("<p onclick=\"steal()\">x</p>"));

        _changed.Should().Be("<p>x</p>");
    }

    [Fact]
    public async Task SavedContent_DropsFormControls()
    {
        // A pasted or stored form could pose as a login prompt inside the content.
        JSInterop.Setup<string>("getHtml", _ => true)
            .SetResult("<p>ok</p><form action=\"https://evil.example\"><input name=\"password\"><button>Sign in</button></form>");
        string? saved = null;

        var cut = Render<WysiwygEditor>(p => p
            .Add(x => x.ShowSaveButton, true)
            .Add(x => x.OnSave, EventCallback.Factory.Create<string>(this, v => saved = v)));

        await cut.Find("[data-mavi-wysiwyg-save]").ClickAsync(new());

        saved.Should().Be("<p>ok</p>");
    }

    [Theory]
    [InlineData("h2")]
    [InlineData("b")]
    [InlineData("i")]
    [InlineData("u")]
    [InlineData("strike")]
    [InlineData("ul > li")]
    [InlineData("ol > li")]
    [InlineData("blockquote")]
    [InlineData("pre")]
    [InlineData("font[face='Georgia'][color='#e03131']")]
    [InlineData("font[style*='font-size']")]
    [InlineData("span[style*='background-color']")]
    [InlineData("div[style*='text-align']")]
    [InlineData("a[href='https://example.com'][target='_blank'][rel='noopener']")]
    [InlineData("a[href='mailto:someone@example.com']")]
    [InlineData("a[href='/docs/page']")]
    [InlineData("img[src^='data:image/png;base64,']")]
    [InlineData("table[style*='border-collapse']")]
    [InlineData("td[style*='padding']")]
    public async Task EditorFormatting_SurvivesSanitization(string selector)
    {
        var cut = RenderTrackingChanges();

        await cut.Instance.SetContentAsync(EDITOR_OUTPUT);

        new HtmlParser().ParseDocument(_changed!).QuerySelectorAll(selector).Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisteredSanitizer_ReplacesTheDefaultPolicy()
    {
        var custom = new HtmlSanitizer();
        custom.AllowedTags.Add("iframe");
        Services.AddSingleton<IHtmlSanitizer>(custom);
        var cut = RenderTrackingChanges();

        await cut.Instance.SetContentAsync("<iframe src=\"https://video.example\"></iframe>");

        _changed.Should().Contain("<iframe");
    }

    private IRenderedComponent<WysiwygEditor> RenderTrackingChanges() =>
        Render<WysiwygEditor>(p => p
            .Add(x => x.ContentChanged, EventCallback.Factory.Create<string?>(this, v => _changed = v)));
}
