using Microsoft.AspNetCore.Components.Web;

namespace Maviray.Blazor.Components.Core.EventArgs;

public class TabClickEventArgs : MouseClickEventArgs
{
    public string? TabNavBarId { get; init; }

    public TabClickEventArgs(string? tabNavBarId, string? tabId) : base(tabId)
    {
        TabNavBarId = tabNavBarId;
    }

    public TabClickEventArgs(string? tabNavBarId, string? tabId, MouseEventArgs args) : base(tabId, args)
    {
        TabNavBarId = tabNavBarId;
    }
}