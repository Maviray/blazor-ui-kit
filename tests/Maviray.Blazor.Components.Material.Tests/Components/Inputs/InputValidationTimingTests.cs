namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     Inputs validate once, when the user commits a value (the change event), never on keystrokes.
///     With <c>Immediate</c> the bound value also follows typing, still without validating.
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
    public void ImmediateStringInput_Typing_UpdatesBoundValue_WithoutValidating()
    {
        string? bound = null;
        var cut = RenderString(v => bound = v, immediate: true);

        cut.Find("input").Input("ab");

        bound.Should().Be("ab");
        _fieldChanges.Should().Be(0);
    }

    [Fact]
    public void ImmediateStringInput_Change_ValidatesFieldOnce()
    {
        var cut = RenderString(_ => { }, immediate: true);

        cut.Find("input").Input("ab");
        cut.Find("input").Change("ab");

        _fieldChanges.Should().Be(1);
    }

    [Fact]
    public void StringInput_Change_ValidatesFieldOnce()
    {
        var cut = RenderString(_ => { }, immediate: false);

        cut.Find("input").Change("ab");

        _fieldChanges.Should().Be(1);
    }

    [Fact]
    public void ImmediateIntegerInput_Typing_UpdatesBoundValue_WithoutValidating()
    {
        var bound = 0;
        var cut = RenderInteger(v => bound = v, immediate: true);

        cut.Find("input").Input("12");

        bound.Should().Be(12);
        _fieldChanges.Should().Be(0);
    }

    [Fact]
    public void ImmediateIntegerInput_Change_ValidatesFieldOnce()
    {
        var cut = RenderInteger(_ => { }, immediate: true);

        cut.Find("input").Input("12");
        cut.Find("input").Change("12");

        _fieldChanges.Should().Be(1);
    }

    [Fact]
    public void IntegerInput_Change_ValidatesFieldOnce()
    {
        var cut = RenderInteger(_ => { }, immediate: false);

        cut.Find("input").Change("12");

        _fieldChanges.Should().Be(1);
    }

    private IRenderedComponent<MaviInputString> RenderString(Action<string> onChanged, bool immediate) =>
        Render<MaviInputString>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Value, _model.Name)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create(this, onChanged))
            .Add(x => x.ValueExpression, () => _model.Name)
            .Add(x => x.Immediate, immediate));

    private IRenderedComponent<MaviInputInteger> RenderInteger(Action<int> onChanged, bool immediate) =>
        Render<MaviInputInteger>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Value, _model.Count)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create(this, onChanged))
            .Add(x => x.ValueExpression, () => _model.Count)
            .Add(x => x.Immediate, immediate));

    private sealed class Model
    {
        public string Name { get; } = string.Empty;
        public int Count { get; }
    }
}
