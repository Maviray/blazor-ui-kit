using System.Globalization;
using Maviray.Blazor.Components.Material.Tests.TestUtilities;

namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     Values are formatted with the formatting culture (<see cref="CultureInfo.CurrentCulture" />), not the
///     UI language culture, and native <c>type="number"</c> inputs always get a browser-parsable value.
/// </summary>
public class InputCultureTests : ComponentTestBase
{
    // .NET formats negative numbers in Swedish with U+2212 MINUS SIGN, which <input type="number"> rejects.
    private static readonly CultureInfo Swedish = new("sv-SE");

    public InputCultureTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void FormattedNumber_UsesFormattingCulture_NotUiCulture()
    {
        using var _ = new CultureScope("de-DE", "en-US");
        var value = 1234.5;

        var cut = Render<MaviInputDouble>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.FormatMask, "N2"));

        cut.Find("input").GetAttribute("value").Should().Be("1.234,50");
    }

    [Fact]
    public void Date_UsesFormattingCulture_NotUiCulture()
    {
        using var _ = new CultureScope("de-DE", "en-US");
        var value = new DateTime(2026, 3, 9);

        var cut = Render<MaviInputDate>(p => p.Bind(x => x.Value, value, v => value = v));

        cut.Find("input").GetAttribute("value").Should().Be("09.03.2026");
    }

    [Fact]
    public void Integer_NegativeValue_IsBrowserParsable()
    {
        var value = -5;

        var cut = Render<MaviInputInteger>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.FormatCulture, Swedish));

        cut.Find("input").Focus();

        AssertNativeNumber(cut.Find("input"), "-5");
    }

    [Fact]
    public void IntegerNullable_NegativeValue_IsBrowserParsable()
    {
        int? value = -5;

        var cut = Render<MaviInputIntegerNullable>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.FormatCulture, Swedish));

        cut.Find("input").Focus();

        AssertNativeNumber(cut.Find("input"), "-5");
    }

    [Fact]
    public void Long_NegativeValue_IsBrowserParsable()
    {
        var value = -5L;

        var cut = Render<MaviInputLong>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.FormatCulture, Swedish));

        cut.Find("input").Focus();

        AssertNativeNumber(cut.Find("input"), "-5");
    }

    [Fact]
    public void Short_NegativeValue_IsBrowserParsable()
    {
        short value = -5;

        var cut = Render<MaviInputShort>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.FormatCulture, Swedish));

        cut.Find("input").Focus();

        AssertNativeNumber(cut.Find("input"), "-5");
    }

    private static void AssertNativeNumber(AngleSharp.Dom.IElement input, string expectedValue)
    {
        input.GetAttribute("type").Should().Be("number");
        input.GetAttribute("value").Should().Be(expectedValue);
    }
}
