using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Samples.Material.Client.Pages;

public class SamplePageBase : ComponentBase
{
    protected const string PRE_CSS = "bg-gray-800 text-white p-4 rounded mt-2 overflow-x-auto w-full max-w-full";
    private ILogger? _logger;

    [Inject]
    private ILoggerFactory? LoggerFactory { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());
}