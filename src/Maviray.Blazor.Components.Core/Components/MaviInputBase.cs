using Maviray.Blazor.Components.Core.Constants;
using Maviray.Blazor.Components.Core.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Core.Components;

public abstract class MaviInputBase<TValue> : InputBase<TValue>
{
    private ILogger? _logger;

    [Inject] private ILoggerFactory? LoggerFactory { get; set; }

    protected ILogger? Logger => _logger ??= LoggerFactory?.CreateLogger(GetType());

    [Parameter] public virtual string? Id { get; set; } = $"input_{Guid.NewGuid()}";
    [Parameter] public string Width { get; set; } = "w-96";

    [Parameter] public string? Style { get; set; }

    [Parameter] public string? Title { get; set; }

    [Parameter] public ElementSize ElementSize { get; set; }

    protected bool HasRendered { get; private set; }

    [Parameter] public string Label { get; set; } = string.Empty;

    [Parameter] public string? Placeholder { get; set; }

    [Parameter] public string? StartIcon { get; set; }

    [Parameter] public string? EndIcon { get; set; }

    [Parameter]
    public bool ShowValidationMessage { get; set; } = true;

    [Parameter]
    public int MaxLength { get; set; } = FormatConstants.DEFAULT_MAX_STRING_INPUT_LIMIT;

    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Readonly { get; set; }
    [Parameter] public bool Required { get; set; }

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    protected bool HasError => EditContext != null && EditContext.GetValidationMessages(FieldIdentifier).Any();

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            HasRendered = true;
        }
    }
}