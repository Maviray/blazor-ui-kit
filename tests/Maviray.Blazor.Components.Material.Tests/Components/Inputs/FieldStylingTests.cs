using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     The floating label and focus colors come from CSS (Tailwind peer variants and per-theme custom
///     properties), so focusing a text field needs no server round-trip. States only the server knows
///     (disabled, readonly, errors, an open list) still set the classes directly.
/// </summary>
public class FieldStylingTests : ComponentTestBase
{
    public FieldStylingTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Theory]
    [InlineData("string")]
    [InlineData("nullable-string")]
    [InlineData("textarea")]
    public void TextInputs_DoNotRoundTripOnFocus(string kind)
    {
        var field = kind switch
        {
            "string" => RenderString().Find("input"),
            "nullable-string" => Render<MaviInputStringNullable>(p => p.Bind(x => x.Value, (string?)null, _ => { })).Find("input"),
            _ => Render<MaviTextArea>(p => p.Bind(x => x.Value, string.Empty, _ => { })).Find("textarea")
        };

        var focusing = () => field.Focus();

        focusing.Should().Throw<MissingEventHandlerException>();
    }

    [Fact]
    public void Label_FloatsThroughCss()
    {
        var cut = RenderString(p => p.Add(x => x.Label, "Name"));

        var input = cut.Find("input");
        input.ClassList.Should().Contain("peer");
        input.GetAttribute("placeholder").Should().Be(" ");
        cut.Find("label").ClassList.Should().Contain(["peer-focus:top-[4px]", "peer-not-placeholder-shown:top-[4px]"]);
    }

    [Fact]
    public void DisabledLabel_AlwaysFloats()
    {
        var cut = RenderString(p => p.Add(x => x.Label, "Name").Add(x => x.Disabled, true));

        var label = cut.Find("label");
        label.ClassList.Should().Contain("top-[5px]");
        label.ClassList.Should().NotContain("peer-focus:top-[4px]");
    }

    [Fact]
    public async Task ErrorColor_IsNotOverriddenByFocus()
    {
        var model = new Model();
        var cut = Render<MaviInputString>(p => p
            .AddCascadingValue(new EditContext(model))
            .Add(x => x.Value, model.Name)
            .Add(x => x.ValueExpression, () => model.Name)
            .Add(x => x.Label, "Name"));

        await cut.InvokeAsync(() => cut.Instance.SetError("Required"));

        var label = cut.Find("label");
        label.ClassList.Should().Contain("text-(--theme-alert-eight)");
        label.ClassList.Should().NotContain("peer-focus:text-(--mavi-field-text-focus)");
    }

    [Fact]
    public void OpenDropdown_FloatsLabelWithoutValue()
    {
        string? value = null;
        var cut = Render<MaviDropdown<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple"])
            .Add(x => x.Label, "Fruit"));

        cut.Find("input").Click();

        cut.Find("label").ClassList.Should().Contain("top-[4px]");
    }

    [Fact]
    public void MultiSelectLabel_DoesNotUseThePlaceholderRule()
    {
        List<string>? value = [];
        var cut = Render<MaviMultiSelect<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple"])
            .Add(x => x.Label, "Fruit"));

        cut.Find("label").GetAttribute("class").Should().NotContain("placeholder-shown");
    }

    [Theory]
    [InlineData(ThemeColorScheme.Success, "--mavi-field-text-focus: var(--theme-success-nine)")]
    [InlineData(ThemeColorScheme.Light, "--mavi-field-border: var(--theme-light-three)")]
    public void ThemeColors_AreCssVariables(ThemeColorScheme scheme, string expectedDeclaration)
    {
        var cut = RenderString(p => p.Add(x => x.ThemeColorScheme, scheme));

        cut.Find("input").ParentElement!.GetAttribute("style").Should().Contain(expectedDeclaration);
    }

    private IRenderedComponent<MaviInputString> RenderString(Action<ComponentParameterCollectionBuilder<MaviInputString>>? configure = null) =>
        Render<MaviInputString>(p =>
        {
            p.Bind(x => x.Value, string.Empty, _ => { });
            configure?.Invoke(p);
        });

    private sealed class Model
    {
        public string Name { get; } = string.Empty;
    }
}
