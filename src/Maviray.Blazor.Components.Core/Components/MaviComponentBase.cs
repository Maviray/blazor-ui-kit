using Maviray.Blazor.Components.Core.Extensions;
using Maviray.Blazor.Components.Core.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Core.Components;

public abstract class MaviComponentBase : ComponentBase
{
    private ILogger? _logger;
    protected bool EnableLifeCycleLogging { get; private set; }

    [Inject]
    private ILoggerFactory? LoggerFactory { get; set; }

    [Inject]
    protected IMaviComponentOptions? ComponentOptions { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());

    [Parameter]
    public virtual string? Id { get; set; } = Guid.NewGuid().ToString();

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public string? Title { get; set; }

    protected bool HasRendered { get; private set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (ComponentOptions != null)
        {
            EnableLifeCycleLogging = ComponentOptions.EnableLifecycleLogging;
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            HasRendered = true;
        }
    }
}