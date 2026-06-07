using Maviray.Blazor.Components.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Maviray.Blazor.Components.Core.Components;

public class MavirayLayoutBase : LayoutComponentBase
{
    [Inject]
    protected ICircuitStateService? CircuitStateService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    private ILogger? _logger;

    [Inject] private ILoggerFactory? LoggerFactory { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());
}