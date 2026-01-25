using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Core.Components;

public class MaviComponentBase : ComponentBase
{
    private ILogger? _logger;

    [Inject]
    private ILoggerFactory? LoggerFactory { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());

    [Parameter]
    public string? Id { get; set; } = System.Guid.NewGuid().ToString();

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public object? Tag { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?> UserAttributes { get; set; } = new();

    protected bool HasRendered { get; private set; }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            HasRendered = true;
        }
    }
}