using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Extensions;
using Maviray.Blazor.Components.Material.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Material.Components.Inputs;

/// <summary>
///     Abstract base class for all Mavi input components.
///     Contains all shared C# logic for validation, theming, event handling, and CSS class building.
/// </summary>
/// <typeparam name="TValue">The type of the input value</typeparam>
public abstract class MaviMaterialInputBase<TValue> : MaviInputBase<TValue>, IAsyncDisposable
{
    #region Protected Properties

    protected bool IsLabelFloating => IsFocused || HasValue || Disabled || Readonly;

    #endregion

    #region Accessibility Helpers

    protected virtual string? GetTitle() => !string.IsNullOrEmpty(Title) ? Title : HelperText;

    #endregion

    #region Theming

    protected virtual void SetThemeColors()
    {
        var colorName = ThemeColorScheme switch
        {
            ThemeColorScheme.Default => "default",
            ThemeColorScheme.Primary => "primary",
            ThemeColorScheme.Secondary => "secondary",
            ThemeColorScheme.Success => "success",
            ThemeColorScheme.Alert => "alert",
            ThemeColorScheme.Warning => "warning",
            ThemeColorScheme.Info => "info",
            ThemeColorScheme.Dark => "dark",
            ThemeColorScheme.Light => "light",
            _ => "primary"
        };

        ThemeTextLightColor = $"text-(--theme-{colorName}-eight)";
        ThemeTextDarkColor = $"text-(--theme-{colorName}-nine)";
        ThemeBorderLightColor = $"border-(--theme-{colorName}-eight)";
        ThemeBorderDarkColor = $"border-(--theme-{colorName}-nine)";
        ThemeBorderHoverColor = $"hover:border-(--theme-{colorName}-nine)";

        // Light scheme: the shared eight/nine formula makes the border read as a solid
        // mid-gray, too close to the Dark variant. Shift only the border ramp lighter
        // (resting < hover < focus), leaving label/icon text on the eight/nine text
        // shades above so it stays legible.
        if (ThemeColorScheme == ThemeColorScheme.Light)
        {
            ThemeBorderLightColor = Tailwind.Theme.Colors.Border.THEME_LIGHT_THREE_BORDER;
            ThemeBorderDarkColor = Tailwind.Theme.Colors.Border.THEME_LIGHT_SEVEN_BORDER;
            ThemeBorderHoverColor = Tailwind.Theme.Colors.Border.Hover.THEME_LIGHT_FIVE_BORDER_HOVER;
        }
    }

    #endregion

    #region Private Helpers

    protected virtual void DetachValidationStateChangedListener()
    {
        if (PreviousEditContext != null)
        {
            PreviousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
            PreviousEditContext.OnFieldChanged -= OnFieldChangedClearManualError;
        }
    }

    #endregion

    #region Protected Fields

    protected bool IsFocused;
    protected bool AttributesInferred;
    protected bool IsEndIconLoading;
    private bool _hasManualError;

    protected readonly string TextAlertColor = Tailwind.Theme.Colors.Text.THEME_ALERT_EIGHT_TEXT;
    protected readonly string BorderAlertColor = Tailwind.Theme.Colors.Border.THEME_ALERT_EIGHT_BORDER;

    protected string ThemeTextLightColor = Tailwind.Theme.Colors.Text.THEME_PRIMARY_EIGHT_TEXT;
    protected string ThemeTextDarkColor = Tailwind.Theme.Colors.Text.THEME_PRIMARY_NINE_TEXT;
    protected string ThemeBorderLightColor = Tailwind.Theme.Colors.Border.THEME_PRIMARY_EIGHT_BORDER;
    protected string ThemeBorderDarkColor = Tailwind.Theme.Colors.Border.THEME_PRIMARY_NINE_BORDER;
    protected string ThemeBorderHoverColor = Tailwind.Theme.Colors.Border.Hover.THEME_PRIMARY_NINE_BORDER_HOVER;

    #endregion

    #region Common Parameters

    [Parameter]
    public string? StartIcon { get; set; }

    /// <summary>
    ///     Icon displayed at the end (right side) of the input field. This icon is clickable.
    /// </summary>
    [Parameter]
    public string? EndIcon { get; set; }

    /// <summary>
    ///     Icon displayed at the end (right side) of the input field. This icon is toggled on-click.
    /// </summary>
    [Parameter]
    public string? EndIconAlternative { get; set; }

    /// <summary>
    ///     Callback invoked when the end icon is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseClickEventArgs> OnEndIconClick { get; set; }

    /// <summary>
    ///     Disables the end icon button independently of the input field.
    /// </summary>
    [Parameter]
    public bool EndIconDisabled { get; set; }

    /// <summary>
    ///     Aria label for the end icon button for accessibility.
    /// </summary>
    [Parameter]
    public string ButtonLabel { get; set; } = "Action";

    /// <summary>
    ///     When true, automatically infers Label, Required, and HelperText from model property attributes.
    ///     Default is true. Set to false in order to disable automatic inference.
    /// </summary>
    [Parameter]
    public bool InferFromModelAttributes { get; set; } = true;

    /// <summary>
    ///     When true, the bound value updates on every keystroke instead of when the user commits the
    ///     value (leaves the field or presses Enter). Each keystroke is then a server call in Blazor Server.
    ///     Default is false.
    /// </summary>
    [Parameter]
    public bool Immediate { get; set; }

    #endregion

    #region Lifecycle Methods

    protected override void OnParametersSet()
    {
        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        base.OnParametersSet();

        SetThemeColors();

        // Only infer once and only if enabled
        if (InferFromModelAttributes && !AttributesInferred)
        {
            InferValuesFromAttributes();
            AttributesInferred = true;
        }

        // Subscribe to EditContext validation state changes
        if (EditContext != PreviousEditContext)
        {
            DetachValidationStateChangedListener();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
                EditContext.OnFieldChanged += OnFieldChangedClearManualError;
                ValidationMessageStore = new(EditContext);
            }

            PreviousEditContext = EditContext;
        }

        // Clear validation messages for disabled/readonly fields
        if ((Disabled || Readonly) && EditContext != null && ValidationMessageStore != null)
        {
            ValidationMessageStore.Clear(FieldIdentifier);
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

    /// <summary>
    ///     The single disposal entry point. Blazor calls only <see cref="DisposeAsync" /> when a component
    ///     implements both disposal interfaces, so this also runs <see cref="InputBase{TValue}" />'s own
    ///     <see cref="IDisposable.Dispose" />, which detaches its EditContext handler and then calls
    ///     <see cref="Dispose(bool)" />.
    ///     Override <see cref="DisposeAsyncCore" /> for JS cleanup and <see cref="Dispose(bool)" /> for the rest.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        ((IDisposable)this).Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (EnableLifeCycleLogging)
            {
                Logger?.LogDebugLifeCycle(Id, GetType());
            }

            DetachValidationStateChangedListener();
        }

        base.Dispose(disposing);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    ///     Called when the EditContext's validation state changes (e.g., on form submit)
    /// </summary>
    protected void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        // Clear validation for disabled/readonly fields
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ((Disabled || Readonly) && EditContext != null && ValidationMessageStore != null)
        {
            ValidationMessageStore.Clear(FieldIdentifier);
        }

        // Re-render the component to reflect the new validation state
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    ///     Handles input event - updates value for UI purposes (label float) without triggering validation
    /// </summary>
    protected abstract Task HandleInput(ChangeEventArgs e);

    /// <summary>
    ///     Whether keystrokes reach the component (and the bound value) before the value is committed.
    /// </summary>
    protected virtual bool BindOnInput => Immediate;

    /// <summary>
    ///     The attributes splatted onto the input element: <see cref="InputBase{TValue}.AdditionalAttributes" />,
    ///     plus the <c>oninput</c> handler when <see cref="BindOnInput" />. Leaving the handler out entirely is what
    ///     stops the browser sending keystrokes; a null delegate would still be wired up.
    /// </summary>
    protected IReadOnlyDictionary<string, object>? InputAttributes
    {
        get
        {
            if (!BindOnInput)
            {
                return AdditionalAttributes;
            }

            var attributes = AdditionalAttributes is null
                ? new Dictionary<string, object>()
                : new Dictionary<string, object>(AdditionalAttributes);

            // An app-supplied oninput wins, as it did when the handler was declared before the splat.
            attributes.TryAdd("oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleInput));

            return attributes;
        }
    }

    /// <summary>
    ///     Commits a value from the change event and validates the field exactly once. Setting
    ///     <c>CurrentValue</c> notifies the EditContext only when the value changes, so this notifies
    ///     itself when typing (<see cref="Immediate" />) already applied the value.
    /// </summary>
    protected void CommitValue(TValue? value)
    {
        if (EqualityComparer<TValue>.Default.Equals(value, Value))
        {
            EditContext?.NotifyFieldChanged(FieldIdentifier);
            return;
        }

        CurrentValue = value;
    }

    /// <summary>
    ///     <see cref="CommitValue" /> for text that still needs parsing, so InputBase records parse errors.
    /// </summary>
    protected void CommitValueAsString(string? text)
    {
        var before = Value;
        CurrentValueAsString = text;

        // InputBase already notified, unless the text parsed to the value that was bound before.
        if (EqualityComparer<TValue>.Default.Equals(before, Value) && TryParseValueFromString(text, out _, out _))
        {
            EditContext?.NotifyFieldChanged(FieldIdentifier);
        }
    }

    /// <summary>
    ///     Updates the bound value while the user is typing. Setting <c>CurrentValue</c> would also
    ///     notify the EditContext and validate on every keystroke; validation instead waits for
    ///     <see cref="HandleChange" />.
    /// </summary>
    protected void SetValueWhileTyping(TValue? value)
    {
        if (EqualityComparer<TValue>.Default.Equals(value, Value))
        {
            return;
        }

        Value = value;
        _ = ValueChanged.InvokeAsync(value);
    }

    /// <summary>
    ///     Handles change event - triggers validation when user is done editing
    /// </summary>
    protected abstract Task HandleChange(ChangeEventArgs e);

    /// <summary>
    ///     Handles the end icon click event
    /// </summary>
    protected virtual async Task HandleEndIconClick(MouseEventArgs args)
    {
        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        if (Disabled || EndIconDisabled || IsEndIconLoading)
        {
            return;
        }

        try
        {
            // Show loading state
            IsEndIconLoading = true;
            StateHasChanged();

            if (!string.IsNullOrEmpty(EndIcon) && !string.IsNullOrWhiteSpace(EndIconAlternative))
            {
                (EndIcon, EndIconAlternative) = (EndIconAlternative, EndIcon);
            }

            // Invoke custom callback
            if (OnEndIconClick.HasDelegate)
            {
                var customArgs = new MouseClickEventArgs(Id, args);

                await OnEndIconClick.InvokeAsync(customArgs);
            }
        }
        finally
        {
            // Hide loading state
            IsEndIconLoading = false;
            StateHasChanged();
        }
    }

    protected void OnFocus()
    {
        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        IsFocused = true;
        StateHasChanged();
    }

    protected virtual void OnBlur()
    {
        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        IsFocused = false;
        StateHasChanged();
    }

    #endregion

    #region Programmatic Validation

    /// <summary>
    ///     Programmatically marks this field as invalid, applying the same red/alert visual
    ///     state used by built-in validation. Intended for validation performed manually
    ///     (e.g. inside an <c>OnValidSubmit</c> handler) rather than via the EditContext.
    ///     Hold an <c>@ref</c> to the component and call this once the check fails.
    ///     The error is automatically cleared the next time the field value changes
    ///     (see <see cref="ClearError" /> to clear it explicitly).
    /// </summary>
    /// <param name="message">
    ///     The validation message stored against this field. It drives the invalid visual
    ///     state and is surfaced through the EditContext (e.g. to a ValidationSummary).
    /// </param>
    public void SetError(string message)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (EditContext == null || ValidationMessageStore == null)
        {
            return;
        }

        ValidationMessageStore.Clear(FieldIdentifier);
        ValidationMessageStore.Add(FieldIdentifier, message);
        _hasManualError = true;

        EditContext.NotifyValidationStateChanged();
    }

    /// <summary>
    ///     Clears a programmatically-set error for this field (see <see cref="SetError" />)
    ///     and removes the invalid visual state.
    /// </summary>
    public void ClearError()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (EditContext == null || ValidationMessageStore == null || !_hasManualError)
        {
            return;
        }

        _hasManualError = false;
        ValidationMessageStore.Clear(FieldIdentifier);

        EditContext.NotifyValidationStateChanged();
    }

    /// <summary>
    ///     Clears a manually-set error as soon as the user edits the field, so the
    ///     programmatic error behaves like built-in validation (which re-validates on change).
    /// </summary>
    private void OnFieldChangedClearManualError(object? sender, FieldChangedEventArgs e)
    {
        if (!_hasManualError || ValidationMessageStore == null)
        {
            return;
        }

        if (!e.FieldIdentifier.Equals(FieldIdentifier))
        {
            return;
        }

        _hasManualError = false;
        ValidationMessageStore.Clear(FieldIdentifier);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        EditContext?.NotifyValidationStateChanged();
    }

    #endregion

    #region Attribute Inference

    protected virtual void InferValuesFromAttributes()
    {
        try
        {
            if (ValueExpression == null)
            {
                return;
            }

            var propertyInfo = GetPropertyInfo();

            if (propertyInfo == null)
            {
                return;
            }

            // Infer Label if not explicitly set
            if (string.IsNullOrEmpty(Label))
            {
                var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
                var displayNameAttr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();

                Label = displayAttr?.Name
                        ?? displayAttr?.GetName()
                        ?? displayNameAttr?.DisplayName
                        ?? SplitCamelCase(propertyInfo.Name);
            }

            // Infer Required if not explicitly set (default is false)
            if (!Required)
            {
                Required = propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
            }

            // Allow derived classes to infer type-specific attributes
            InferTypeSpecificAttributes(propertyInfo);
        }
        catch (Exception ex)
        {
            Logger?.LogWarning(ex, "Failed to infer values from model attributes for property '{PropertyName}'",
                GetPropertyInfo()?.Name ?? "Unknown");
        }
    }

    /// <summary>
    ///     Override in derived classes to infer type-specific attributes (e.g., Range, MaxLength, etc.)
    /// </summary>
    protected virtual void InferTypeSpecificAttributes(PropertyInfo propertyInfo)
    {
        // Default: no type-specific inference
    }

    protected PropertyInfo? GetPropertyInfo()
    {
        if (ValueExpression == null)
        {
            return null;
        }

        var expression = ValueExpression.Body;

        return expression switch
        {
            // Handle member access (e.g., () => model.Property)
            MemberExpression memberExpression => memberExpression.Member as PropertyInfo,
            // Handle converted expressions (e.g., () => (object)model.Property)
            UnaryExpression { Operand: MemberExpression operandMember } => operandMember.Member as PropertyInfo,
            _ => null
        };
    }

    protected static string SplitCamelCase(string input) => string.IsNullOrEmpty(input) ? input : Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");

    #endregion

    #region CSS Class Builders (Shared Across All Components)

    protected virtual string GetContainerClass()
    {
        var heightClass = ElementSize switch
        {
            ElementSize.Small => "h-10", // 40px
            ElementSize.Large => "h-14", // 56px
            _ => "h-12" // 48px (Regular)
        };

        return $"relative mt-[2px] {heightClass}";
    }

    protected virtual string BuildInputClass()
    {
        var classes = new List<string>
        {
            "block w-full h-full bg-transparent border-0 outline-none rounded transition-all duration-200"
        };

        // Text color
        if (Disabled)
        {
            classes.Add("cursor-not-allowed text-black/[0.38]");
        }
        else if (Readonly)
        {
            classes.Add("cursor-default text-black/[0.87]");
        }
        else
        {
            classes.Add("text-black/[0.87]");
        }

        // Placeholder - should be transparent since we use floating label
        classes.Add("placeholder:text-transparent");

        // Focus
        classes.Add("focus:outline-none focus:ring-0");

        // Padding and font size for each size
        var (padding, fontSize, lineHeight) = ElementSize switch
        {
            ElementSize.Small => GetSizePadding("px-3 pt-4 pb-2", "pl-[36px] pr-3 pt-[12px] pb-2", "pr-[36px]", "pl-[36px] pr-[36px] pt-4 pb-2", "text-sm", "leading-normal"),
            ElementSize.Large => GetSizePadding("px-4 pt-5 pb-3", "pl-[38px] pr-4 pt-[14px] pb-3", "pr-[44px]", "pl-[38px] pr-[44px] pt-5 pb-3", "text-lg", "leading-normal"),
            _ => GetSizePadding("px-3.5 pt-4.5 pb-2.5", "pl-[36px] pr-3.5 pt-[12px] pb-2.5", "pr-[40px]", "pl-[36px] pr-[40px] pt-4.5 pb-2.5", "text-base", "leading-normal")
        };

        classes.Add(padding);
        classes.Add(fontSize);
        classes.Add(lineHeight);

        return string.Join(" ", classes);
    }

    protected (string padding, string fontSize, string lineHeight) GetSizePadding(
        string defaultPadding,
        string iconPadding,
        string endIconPadding,
        string bothIconsPadding,
        string fontSize,
        string lineHeight)
    {
        var hasStartIcon = !string.IsNullOrEmpty(StartIcon);
        var hasEndIcon = !string.IsNullOrEmpty(EndIcon);

        var padding = (hasStartIcon, hasEndIcon) switch
        {
            (true, true) => bothIconsPadding,
            (true, false) => iconPadding,
            (false, true) => $"px-3.5 pt-4.5 pb-2.5 {endIconPadding}",
            _ => defaultPadding
        };

        return (padding, fontSize, lineHeight);
    }

    protected virtual string BuildLabelClass()
    {
        var classes = new List<string>
        {
            "absolute transition-all duration-200 pointer-events-none leading-none"
        };

        // Position based on floating state
        if (IsLabelFloating)
        {
            if (Disabled || Readonly)
            {
                classes.Add("top-[5px]");
            }
            else
            {
                classes.Add("top-[4px]");
            }

            // Floating: positioned on top of the border, centered on the border
            classes.Add("-translate-y-1/2 left-[11px] px-1 text-xs");
        }
        else
        {
            // Not floating: perfectly centered vertically and horizontally inside the field
            var (left, fontSize) = ElementSize switch
            {
                ElementSize.Small => (!string.IsNullOrEmpty(StartIcon) ? "left-[37px]" : "left-3", "text-sm"),
                ElementSize.Large => (!string.IsNullOrEmpty(StartIcon) ? "left-[38px]" : "left-4", "text-base"),
                _ => (!string.IsNullOrEmpty(StartIcon) ? "left-[37px]" : "left-3.5", "text-base")
            };

            // Perfect vertical center
            classes.Add($"{GetLabelAsPlaceholderTopOffset()} -translate-y-1/2 {left} {fontSize}");
        }

        // Add border for disabled state when floating
        if (Disabled)
        {
            classes.Add("border-black/[0.12]");
        }

        if (Readonly)
        {
            classes.Add("border-black/[0.24]");
        }

        if (Disabled || Readonly)
        {
            classes.Add("bg-white border border-solid rounded pb-[2px] z-10");
        }

        // Color based on state (matching border state)
        if (HasError)
        {
            classes.Add(TextAlertColor);
        }
        else if (IsFocused)
        {
            classes.Add(ThemeTextDarkColor);
        }
        else if (Disabled)
        {
            classes.Add("text-black/[0.23]");
        }
        else if (Readonly)
        {
            classes.Add("text-black/[0.38]");
        }
        else
        {
            classes.Add(ThemeTextLightColor);
        }

        return string.Join(" ", classes);
    }

    protected virtual string GetLabelAsPlaceholderTopOffset()
    {
        var topOffset = ElementSize switch
        {
            ElementSize.Small => "top-[22px]",
            ElementSize.Large => "top-[30px]",
            _ => "top-[25px]"
        };

        return topOffset;
    }

    protected virtual string BuildFieldsetClass()
    {
        var classes = new List<string>
        {
            "absolute inset-0 pointer-events-none rounded border border-solid m-0 p-0 transition-all duration-200"
        };

        // Border width - thicker when focused or error
        if (IsFocused || HasError)
        {
            classes.Add("!border-2");
        }

        // Border color based on state
        if (HasError)
        {
            // Error state - always red
            classes.Add(BorderAlertColor);
        }
        else if (IsFocused)
        {
            // Focused/Active state - darker primary
            classes.Add(ThemeBorderDarkColor);
        }
        else if (Disabled)
        {
            // Disabled state
            classes.Add("border-black/[0.12]");
            classes.Add("bg-black/[0.04]");
        }
        else if (Readonly)
        {
            // Readonly state
            classes.Add("border-black/[0.23]");
            classes.Add("bg-black/[0.04]");
        }
        else
        {
            // Default state with hover
            classes.Add(ThemeBorderLightColor);
            classes.Add(ThemeBorderHoverColor);
        }

        return string.Join(" ", classes);
    }

    protected virtual string BuildLegendClass()
    {
        var classes = new List<string>
        {
            "px-0 text-xs transition-all duration-200 invisible whitespace-nowrap leading-none",
            // Show legend when label is floating to create cutout in border
            Disabled || Readonly ? "ml-2.5" : "ml-1.5",
            IsLabelFloating ? "max-w-full" : "max-w-0"
        };

        return string.Join(" ", classes);
    }

    protected virtual string BuildRequiredAsteriskClass() => $"ml-0.5 {TextAlertColor}";

    protected virtual string GetStartIconClass()
    {
        var classes = new List<string>
        {
            $"absolute flex {GetIconTopOffset()} -translate-y-1/2 left-3 pointer-events-none transition-colors duration-200 z-10"
        };

        // Icon color based on state
        if (HasError)
        {
            classes.Add(TextAlertColor);
        }
        else if (IsFocused)
        {
            classes.Add(ThemeTextDarkColor);
        }
        else if (Disabled)
        {
            classes.Add("text-black/[0.23]");
        }
        else if (Readonly)
        {
            classes.Add("text-black/[0.38]");
        }
        else
        {
            classes.Add(ThemeTextLightColor);
        }

        return string.Join(" ", classes);
    }

    protected virtual string GetEndIconClass()
    {
        var classes = new List<string>
        {
            $"absolute {GetIconTopOffset()} -translate-y-1/2 right-[8px] transition-colors duration-200 z-10",
            "flex items-center justify-center rounded-full",
            "focus:outline-none"
        };

        // Button states
        if (Disabled || EndIconDisabled)
        {
            classes.Add("cursor-not-allowed text-black/[0.38]");
        }
        else
        {
            classes.Add("cursor-pointer hover:bg-black/[0.04] active:bg-black/[0.1]");

            // Icon color based on state
            if (HasError)
            {
                classes.Add(TextAlertColor);
            }
            else if (IsFocused)
            {
                classes.Add(ThemeTextDarkColor);
            }
            else
            {
                classes.Add(ThemeTextLightColor);
            }
        }

        // Size based on element size
        var buttonSize = ElementSize switch
        {
            ElementSize.Small => "w-6 h-6",
            ElementSize.Large => "w-8 h-8",
            _ => "w-7 h-7"
        };

        classes.Add(buttonSize);

        return string.Join(" ", classes);
    }

    protected virtual string GetEndIconLoadingClass()
    {
        var classes = new List<string>
        {
            $"absolute {GetIconTopOffset()} -translate-y-1/2 right-[8px] z-10",
            "flex items-center justify-center"
        };

        // Loading spinner color based on state
        if (HasError)
        {
            classes.Add(TextAlertColor);
        }
        else if (IsFocused)
        {
            classes.Add(ThemeTextDarkColor);
        }
        else
        {
            classes.Add(ThemeTextLightColor);
        }

        return string.Join(" ", classes);
    }

    protected virtual string GetLoadingSpinnerSize()
    {
        var size = ElementSize switch
        {
            ElementSize.Small => "width: 16px; height: 16px;",
            ElementSize.Large => "width: 20px; height: 20px;",
            _ => "width: 18px; height: 18px;"
        };

        return size;
    }

    protected virtual string GetIconTopOffset()
    {
        var topOffset = ElementSize switch
        {
            ElementSize.Small => "top-[23px]",
            ElementSize.Large => "top-[31px]",
            _ => "top-[27px]"
        };

        return topOffset;
    }

    #endregion
}