using Maviray.Blazor.Components.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Core.Components;

public class MavirayLayoutBase : LayoutComponentBase
{
    private ILogger? _logger;

    [Inject]
    protected ICircuitStateService? CircuitStateService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    private ILoggerFactory? LoggerFactory { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());
}