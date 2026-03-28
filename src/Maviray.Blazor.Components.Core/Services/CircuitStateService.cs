using Maviray.Blazor.Components.Core.EventArgs;

namespace Maviray.Blazor.Components.Core.Services;

public class CircuitStateService : ICircuitStateService
{
    public event EventHandler<ButtonClickEventArgs>? ButtonClicked;

    // Public method to report click
    public virtual void ReportClick(string buttonId, string? additionalData = null)
    {
        var args = new ButtonClickEventArgs(buttonId);

        ButtonClicked?.Invoke(this, args);
    }
}