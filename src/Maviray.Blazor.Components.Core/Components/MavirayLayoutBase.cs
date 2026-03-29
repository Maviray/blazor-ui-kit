using Maviray.Blazor.Components.Core.Services;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Core.Components;

public class MavirayLayoutBase : LayoutComponentBase
{
    [Inject]
    protected ICircuitStateService? CircuitStateService { get; set; }
}