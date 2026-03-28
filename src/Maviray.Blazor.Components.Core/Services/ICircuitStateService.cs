using Maviray.Blazor.Components.Core.EventArgs;

namespace Maviray.Blazor.Components.Core.Services;

public interface ICircuitStateService
{
    public event EventHandler<ButtonClickEventArgs>? ButtonClicked;
    public void ReportClick(string buttonId, string? additionalData = null);
}