namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     Inputs update their bound value when the user commits it (the change event), like Blazor's
///     built-in inputs, so keystrokes are never sent to the server. <c>Immediate</c> opts back in.
/// </summary>
public class InputBindingTests : ComponentTestBase
{
    [Fact]
    public void StringInput_Typing_IsNotSentToTheComponent()
    {
        var value = string.Empty;
        var cut = Render<MaviInputString>(p => p.Bind(x => x.Value, value, v => value = v));

        var typing = () => cut.Find("input").Input("ab");

        typing.Should().Throw<MissingEventHandlerException>();
    }

    [Fact]
    public void IntegerInput_Typing_IsNotSentToTheComponent()
    {
        var value = 0;
        var cut = Render<MaviInputInteger>(p => p.Bind(x => x.Value, value, v => value = v));

        var typing = () => cut.Find("input").Input("12");

        typing.Should().Throw<MissingEventHandlerException>();
    }

    [Fact]
    public void StringInput_Change_UpdatesBoundValue()
    {
        var value = string.Empty;
        var cut = Render<MaviInputString>(p => p.Bind(x => x.Value, value, v => value = v));

        cut.Find("input").Change("ab");

        value.Should().Be("ab");
    }

    [Fact]
    public void ImmediateStringInput_Typing_UpdatesBoundValue()
    {
        var value = string.Empty;
        var cut = Render<MaviInputString>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Immediate, true));

        cut.Find("input").Input("ab");

        value.Should().Be("ab");
    }

    [Fact]
    public void TextAreaWithCharacterCount_UpdatesWhileTyping()
    {
        var value = string.Empty;
        var cut = Render<MaviTextArea>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.ShowCharacterCount, true));

        cut.Find("textarea").Input("abc");

        value.Should().Be("abc");
    }
}
