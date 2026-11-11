using Maviray.Blazor.Components.Material.Tests.TestUtilities;
using Microsoft.JSInterop;

namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     A disposed input must not stay attached to the form's EditContext: the form outlives
///     conditionally rendered fields, so every leaked handler keeps a dead component alive
///     and is invoked on each later validation.
/// </summary>
public class InputDisposalTests : ComponentTestBase
{
    private const string DATE_PICKER_MODULE = "./_content/Maviray.Blazor.Components.Material/js/maviDatePickerJsInterop.js";

    private readonly Model _model = new();
    private readonly EditContext _editContext;

    public InputDisposalTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        _editContext = new(_model);
    }

    [Fact]
    public async Task StringInput_DetachesFromEditContext_WhenDisposed()
    {
        Render<MaviInputString>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Value, _model.Name)
            .Add(x => x.ValueExpression, () => _model.Name));

        await DisposeComponentsAsync();

        AssertNoEditContextHandlers();
    }

    [Fact]
    public async Task Dropdown_DetachesFromEditContext_WhenDisposed()
    {
        Render<MaviDropdown<string, string>>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Items, ["a", "b"])
            .Add(x => x.Value, _model.Name)
            .Add(x => x.ValueExpression, () => _model.Name));

        await DisposeComponentsAsync();

        AssertNoEditContextHandlers();
    }

    [Fact]
    public async Task DateInput_DetachesFromEditContext_WhenDisposed()
    {
        Render<MaviInputDate>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Value, _model.Date)
            .Add(x => x.ValueExpression, () => _model.Date));

        await DisposeComponentsAsync();

        AssertNoEditContextHandlers();
    }

    [Fact]
    public async Task DateInput_Dispose_ToleratesDisconnectedCircuit()
    {
        var module = JSInterop.SetupModule(DATE_PICKER_MODULE);
        module.SetupVoid("disposeDatePicker", _ => true).SetException(new JSDisconnectedException("Circuit is gone."));

        var cut = Render<MaviInputDate>(p => p
            .AddCascadingValue(_editContext)
            .Add(x => x.Value, _model.Date)
            .Add(x => x.ValueExpression, () => _model.Date));

        // Open the calendar so there is a click listener to detach at disposal.
        cut.Find("input").Click();
        cut.WaitForAssertion(() => module.Invocations["initializeDatePicker"].Should().HaveCount(1));

        await DisposeComponentsAsync();

        // The renderer reports disposal failures here instead of throwing them.
        module.Invocations["disposeDatePicker"].Should().HaveCount(1);
        Renderer.UnhandledException.IsCompleted.Should().BeFalse();
    }

    private void AssertNoEditContextHandlers()
    {
        EventSubscriptions.Count(_editContext, nameof(EditContext.OnValidationStateChanged)).Should().Be(0);
        EventSubscriptions.Count(_editContext, nameof(EditContext.OnFieldChanged)).Should().Be(0);
    }

    private sealed class Model
    {
        public string Name { get; } = string.Empty;
        public DateTime Date { get; }
    }
}
