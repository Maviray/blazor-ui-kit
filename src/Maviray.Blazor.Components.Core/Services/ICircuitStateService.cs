using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Interfaces;

namespace Maviray.Blazor.Components.Core.Services;

/// <summary>
///     this service is designed to contain "session" status - i.e. everything related to user session. In Blazor - a
///     circuit is equivalent of a session (one circuit per browser page), so this service is to be registered as scoped.
/// </summary>
public interface ICircuitStateService
{
    event EventHandler<ButtonClickEventArgs>? ButtonClicked;

    void ReportClick(string? buttonId);

    Task<IEnumerable<IMenuItem>> GetMenuItems();
}