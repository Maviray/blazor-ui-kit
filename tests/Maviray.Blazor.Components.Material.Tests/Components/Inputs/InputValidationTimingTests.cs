namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     Inputs validate when the user commits a value (the change event), not on every keystroke.
///     The bound value still follows typing so consumers can react live.
/// </summary>
public class InputValidationTimingTests : ComponentTestBase
{
    private readonly Model _model = new();
    private readonly EditContext _editContext;
    private int _fieldChanges;

    public InputValidationTimingTests()
    {
        _editContext = new(_model);
        _editContext.OnFieldChanged += (_, _) => _fieldChanges++;
    }

    [Fact]
    public void StringInput_Typing_UpdatesBoundValue_WithoutValidating()
    {
        string? bound = null;
        var cut = RenderString(v => bound = v);

        cut.Find("input").Input("ab");

        bound.Should().Be("ab");
        _fieldChanges.Should().Be(0);
    }

    [Fact]
    public void StringInput_Change_ValidatesFieldOnce()
    {
        var cut = RenderString(_ => { });

        cut.Find("input").Input("ab");
        cut.Find("input").Change("ab");

        _fieldChanges.Should().Be(1);
    }

    [Fact]
    public void IntegerInput_Typing_UpdatesBoundValue_WithoutValidating()
    {
        var bound = 0;
        var cut = RenderInteger(v => bound = v);

        cut.Find("input").Input("12");

        bound.Should().Be(12);
        _fieldChanges.Should().Be(0);
    }

    [Fact]
    public void IntegerInput_Change_ValidatesFieldOnce()
    {
        var cut = RenderInteger(_ => { });

        cut.Find("input").Input("12");
        cut.Find("input").Change("12");

        _fieldChanges.Should().Be(1);
    }

    private IRenderedComponent<MaviInputString> RenderString(Action<string> onChanged) =>
        Render<MaviInputString>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Value, _model.Name)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create(this, onChanged))
            .Add(x => x.ValueExpression, () => _model.Name));

    private IRenderedComponent<MaviInputInteger> RenderInteger(Action<int> onChanged) =>
        Render<MaviInputInteger>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Value, _model.Count)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create(this, onChanged))
            .Add(x => x.ValueExpression, () => _model.Count));

    private sealed class Model
    {
        public string Name { get; } = string.Empty;
        public int Count { get; }
    }
}
