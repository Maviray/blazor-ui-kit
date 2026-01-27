using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Samples.Material.Client.Pages;

public class SamplePageBase : ComponentBase
{
    private ILogger? _logger;

    [Inject] private ILoggerFactory? LoggerFactory { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());
}