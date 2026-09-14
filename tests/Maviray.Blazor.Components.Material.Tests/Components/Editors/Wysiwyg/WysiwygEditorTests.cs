using Bunit;
using Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

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
}
