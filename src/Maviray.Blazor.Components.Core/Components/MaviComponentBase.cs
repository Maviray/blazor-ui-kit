using Maviray.Blazor.Components.Core.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Maviray.Blazor.Components.Core.Extensions;

namespace Maviray.Blazor.Components.Core.Components;

public abstract class MaviComponentBase : ComponentBase
{
    private ILogger? _logger;

    [Inject] private ILoggerFactory? LoggerFactory { get; set; }

    [Inject] protected IMaviComponentOptions? ComponentOptions { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());

    [Parameter] public virtual string? Id { get; set; } = System.Guid.NewGuid().ToString();

    [Parameter] public string? Class { get; set; }

    [Parameter] public string? Style { get; set; }

    [Parameter] public string? Title { get; set; }

    protected bool HasRendered { get; private set; }

    protected override void OnAfterRender(bool firstRender)
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            HasRendered = true;
        }
    }
}