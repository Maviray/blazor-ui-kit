using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Interfaces;

namespace Maviray.Blazor.Components.Core.Services;

/// <summary>
/// this service is designed to contain "session" status - i.e. everything related to user session. In Blazor - a circuit is equivalent of a session (one circuit per browser page), so this service is to be registered as scoped. 
/// </summary>
public class CircuitStateService : ICircuitStateService
{
    public event EventHandler<ButtonClickEventArgs>? ButtonClicked;

    // Override this method in derived class to build relevant menu by injecting for instance Identity Principal
    public virtual Task<IEnumerable<IMenuItem>> GetMenuItems()
    {
        return Task.FromResult(Enumerable.Empty<IMenuItem>());
    }

    // Public method to report click
    public virtual void ReportClick(string? buttonId)
    {
        var args = new ButtonClickEventArgs(buttonId);

        ButtonClicked?.Invoke(this, args);
    }
}