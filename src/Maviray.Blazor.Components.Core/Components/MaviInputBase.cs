using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Core.Components;

public abstract class MaviInputBase<TValue> : InputBase<TValue>
{
    protected EditContext? PreviousEditContext;
    protected ValidationMessageStore? ValidationMessageStore;

    [Inject]
    private ILoggerFactory? LoggerFactory { get; set; }

    [Inject]
    protected IMaviComponentOptions? ComponentOptions { get; set; }

    protected bool EnableLifeCycleLogging { get; private set; }

    public bool HasRendered { get; protected set; }

    protected ILogger? Logger => field ??= LoggerFactory?.CreateLogger(GetType());

    /// <summary>
    ///     Unique identifier for the input component
    /// </summary>
    [Parameter]
    public virtual string? Id { get; set; } = $"input_{Guid.NewGuid()}";

    /// <summary>
    ///     Width CSS class (e.g., "w-96", "w-full")
    /// </summary>
    [Parameter]
    public string Width { get; set; } = "w-auto";

    /// <summary>
    ///     Inline styles to apply to the component
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    ///     Title/tooltip text
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    ///     Theme color scheme for the component
    /// </summary>
    [Parameter]
    public ThemeColorScheme ThemeColorScheme { get; set; } = ThemeColorScheme.Primary;

    /// <summary>
    ///     Size of element (e.g., Small, Regular, Large)
    /// </summary>
    [Parameter]
    public ElementSize ElementSize { get; set; }

    /// <summary>
    ///     Label text for the input
    /// </summary>
    [Parameter]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    ///     Helper/hint text displayed below the input
    /// </summary>
    [Parameter]
    public string? HelperText { get; set; }

    /// <summary>
    ///     Indicates if the field is required
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    ///     Disables the input
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    ///     Makes the input read-only
    /// </summary>
    [Parameter]
    public bool Readonly { get; set; }

    /// <summary>
    ///     Additional CSS classes to apply
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    protected bool HasError => EditContext != null && !Disabled && !Readonly && EditContext.GetValidationMessages(FieldIdentifier).Any();

    protected abstract bool HasValue { get; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (ComponentOptions != null)
        {
            EnableLifeCycleLogging = ComponentOptions.EnableLifecycleLogging;
        }
    }

    protected virtual string GetAriaDescribedByString() => !string.IsNullOrEmpty(HelperText) ? $"{Id}-helper" : string.Empty;

    protected virtual string GetAreaInvalidValue() => HasError.ToString().ToLowerInvariant();
}