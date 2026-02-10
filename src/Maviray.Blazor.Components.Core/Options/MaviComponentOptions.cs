using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Core.Options;

public class MaviComponentOptions : IMaviComponentOptions
{
    public LogLevel ComponentLogLevel { get; set; } = LogLevel.Warning;
    public bool EnableLifecycleLogging { get; set; } = false;
    public bool EnablePerformanceTracking { get; set; } = false;
}