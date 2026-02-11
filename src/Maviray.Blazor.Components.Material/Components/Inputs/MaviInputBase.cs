using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Options;
using Maviray.Blazor.Components.Material.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Maviray.Blazor.Components.Core.Extensions;

namespace Maviray.Blazor.Components.Material.Components.Inputs;

/// <summary>
/// Abstract base class for all Mavi input components.
/// Contains all shared C# logic for validation, theming, event handling, and CSS class building.
/// </summary>
/// <typeparam name="TValue">The type of the input value</typeparam>
public abstract class MaviInputBase<TValue> : InputBase<TValue>, IDisposable
{
    #region Protected Fields

    protected bool _isFocused;
    protected bool _attributesInferred = false;
    protected EditContext? _previousEditContext;
    protected ValidationMessageStore? _validationMessageStore;
    protected bool _isEndIconLoading = false;

    protected readonly string _textAlertColor = Tailwind.Theme.Colors.Text.THEME_ALERT_EIGHT_TEXT;
    protected readonly string _borderAlertColor = Tailwind.Theme.Colors.Border.THEME_ALERT_EIGHT_BORDER;

    protected string _themeTextLightColor = Tailwind.Theme.Colors.Text.THEME_PRIMARY_EIGHT_TEXT;
    protected string _themeTextDarkColor = Tailwind.Theme.Colors.Text.THEME_ACCENT_NINE_TEXT;
    protected string _themeBorderLightColor = Tailwind.Theme.Colors.Border.THEME_PRIMARY_EIGHT_BORDER;
    protected string _themeBorderDarkColor = Tailwind.Theme.Colors.Border.THEME_PRIMARY_NINE_BORDER;
    protected string _themeBorderHoverColor = Tailwind.Theme.Colors.Border.Hover.THEME_PRIMARY_NINE_BORDER_HOVER;

    #endregion

    #region Injected Services

    [Inject] private ILoggerFactory? LoggerFactory { get; set; }
    [Inject] protected IMaviComponentOptions? ComponentOptions { get; set; }

    protected ILogger? Logger => field ??= LoggerFactory?.CreateLogger(GetType());

    #endregion

    #region Common Parameters

    [Parameter] public virtual string? Id { get; set; } = $"input_{Guid.NewGuid()}";
    [Parameter] public string Width { get; set; } = "w-96";
    [Parameter] public string? Style { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public ElementSize ElementSize { get; set; } = ElementSize.Regular;
    [Parameter] public ThemeColorScheme ThemeColorScheme { get; set; } = ThemeColorScheme.Primary;
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string? StartIcon { get; set; }

    /// <summary>
    /// Icon displayed at the end (right side) of the input field. This icon is clickable.
    /// </summary>
    [Parameter] public string? EndIcon { get; set; }

    /// <summary>
    /// Icon displayed at the end (right side) of the input field. This icon is toggled on-click.
    /// </summary>
    [Parameter] public string? EndIconAlternative { get; set; }

    /// <summary>
    /// Callback invoked when the end icon is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseClickEventArgs> OnEndIconClick { get; set; }

    /// <summary>
    /// Disables the end icon button independently of the input field.
    /// </summary>
    [Parameter] public bool EndIconDisabled { get; set; }

    /// <summary>
    /// Aria label for the end icon button for accessibility.
    /// </summary>
    [Parameter] public string ButtonLabel { get; set; } = "Action";

    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Readonly { get; set; }
    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// When true, automatically infers Label, Required, and HelperText from model property attributes.
    /// Default is true. Set to false to disable automatic inference.
    /// </summary>
    [Parameter] public bool InferFromModelAttributes { get; set; } = true;

    /// <summary>
    /// Helper text displayed below the input field. Provides guidance to the user.
    /// Can be automatically inferred from [Display(Description = "")] or [Display(Prompt = "")] attributes.
    /// </summary>
    [Parameter] public string? HelperText { get; set; }

    #endregion

    #region Protected Properties

    protected bool HasRendered { get; private set; }

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    protected bool HasError => EditContext != null && !Disabled && !Readonly && EditContext.GetValidationMessages(FieldIdentifier).Any();

    protected abstract bool HasValue { get; }
    protected bool IsLabelFloating => _isFocused || HasValue || Disabled || Readonly;

    #endregion

    #region Lifecycle Methods

    protected override void OnParametersSet()
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        base.OnParametersSet();

        SetThemeColors();

        // Only infer once and only if enabled
        if (InferFromModelAttributes && !_attributesInferred)
        {
            InferValuesFromAttributes();
            _attributesInferred = true;
        }

        // Subscribe to EditContext validation state changes
        if (EditContext != _previousEditContext)
        {
            DetachValidationStateChangedListener();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
                _validationMessageStore = new ValidationMessageStore(EditContext);
            }

            _previousEditContext = EditContext;
        }

        // Clear validation messages for disabled/readonly fields
        if ((Disabled || Readonly) && EditContext != null && _validationMessageStore != null)
        {
            _validationMessageStore.Clear(FieldIdentifier);
        }
    }

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

    public virtual void Dispose()
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        DetachValidationStateChangedListener();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when the EditContext's validation state changes (e.g., on form submit)
    /// </summary>
    protected void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        // Clear validation for disabled/readonly fields
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ((Disabled || Readonly) && EditContext != null && _validationMessageStore != null)
        {
            _validationMessageStore.Clear(FieldIdentifier);
        }

        // Re-render the component to reflect the new validation state
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Handles input event - updates value for UI purposes (label float) without triggering validation
    /// </summary>
    protected abstract void HandleInput(ChangeEventArgs e);

    /// <summary>
    /// Handles change event - triggers validation when user is done editing
    /// </summary>
    protected abstract void HandleChange(ChangeEventArgs e);

    /// <summary>
    /// Handles the end icon click event
    /// </summary>
    protected virtual async Task HandleEndIconClick(MouseEventArgs args)
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        if (Disabled || EndIconDisabled || _isEndIconLoading)
            return;

        try
        {
            // Show loading state
            _isEndIconLoading = true;
            StateHasChanged();

            if (!string.IsNullOrEmpty(EndIcon) && !string.IsNullOrWhiteSpace(EndIconAlternative))
            {
                (EndIcon, EndIconAlternative) = (EndIconAlternative, EndIcon);
            }

            // Invoke custom callback
            if (OnEndIconClick.HasDelegate)
            {
                var customArgs = new MouseClickEventArgs(Id)
                {
                    AltKey = args.AltKey,
                    Button = args.Button,
                    Buttons = args.Buttons,
                    ClientX = args.ClientX,
                    ClientY = args.ClientY,
                    CtrlKey = args.CtrlKey,
                    Detail = args.Detail,
                    MetaKey = args.MetaKey,
                    OffsetX = args.OffsetX,
                    OffsetY = args.OffsetY,
                    PageX = args.PageX,
                    PageY = args.PageY,
                    ScreenX = args.ScreenX,
                    ScreenY = args.ScreenY,
                    ShiftKey = args.ShiftKey,
                    Type = args.Type
                };

                await OnEndIconClick.InvokeAsync(customArgs);
            }
        }
        finally
        {
            // Hide loading state
            _isEndIconLoading = false;
            StateHasChanged();
        }
    }

    protected void OnFocus()
    {
        _isFocused = true;
        StateHasChanged();
    }

    protected virtual void OnBlur()
    {
        _isFocused = false;
        StateHasChanged();
    }

    #endregion

    #region Attribute Inference

    protected virtual void InferValuesFromAttributes()
    {
        try
        {
            if (ValueExpression == null)
                return;

            var propertyInfo = GetPropertyInfo();

            if (propertyInfo == null)
                return;

            // Infer Label if not explicitly set
            if (string.IsNullOrEmpty(Label))
            {
                var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
                var displayNameAttr = propertyInfo.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>();

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

            // Infer HelperText if not explicitly set
            if (string.IsNullOrEmpty(HelperText))
            {
                var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
                HelperText = displayAttr?.Description ?? displayAttr?.Prompt;
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
    /// Override in derived classes to infer type-specific attributes (e.g., Range, MaxLength, etc.)
    /// </summary>
    protected virtual void InferTypeSpecificAttributes(PropertyInfo propertyInfo)
    {
        // Default: no type-specific inference
    }

    protected PropertyInfo? GetPropertyInfo()
    {
        if (ValueExpression == null)
            return null;

        var expression = ValueExpression.Body;

        return expression switch
        {
            // Handle member access (e.g., () => model.Property)
            System.Linq.Expressions.MemberExpression memberExpression => memberExpression.Member as PropertyInfo,
            // Handle converted expressions (e.g., () => (object)model.Property)
            System.Linq.Expressions.UnaryExpression { Operand: System.Linq.Expressions.MemberExpression operandMember } => operandMember.Member as PropertyInfo,
            _ => null
        };
    }

    protected static string SplitCamelCase(string input)
    {
        return string.IsNullOrEmpty(input) ? input : System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    }

    #endregion

    #region CSS Class Builders (Shared Across All Components)

    protected string GetContainerClass()
    {
        var heightClass = ElementSize switch
        {
            ElementSize.Small => "h-10",      // 40px
            ElementSize.Large => "h-14",      // 56px
            _ => "h-12"                       // 48px (Regular)
        };

        return $"relative mt-[2px] {heightClass}";
    }

    protected string BuildInputClass()
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

    protected string BuildLabelClass()
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
            classes.Add(_textAlertColor);
        }
        else if (_isFocused)
        {
            classes.Add(_themeTextDarkColor);
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
            classes.Add(_themeTextLightColor);
        }

        return string.Join(" ", classes);
    }

    protected string GetLabelAsPlaceholderTopOffset()
    {
        var topOffset = ElementSize switch
        {
            ElementSize.Small => "top-[22px]",
            ElementSize.Large => "top-[30px]",
            _ => "top-[25px]"
        };

        return topOffset;
    }

    protected string BuildFieldsetClass()
    {
        var classes = new List<string>
        {
            "absolute inset-0 pointer-events-none rounded border border-solid m-0 p-0 transition-all duration-200"
        };

        // Border width - thicker when focused or error
        if (_isFocused || HasError)
        {
            classes.Add("!border-2");
        }

        // Border color based on state
        if (HasError)
        {
            // Error state - always red
            classes.Add(_borderAlertColor);
        }
        else if (_isFocused)
        {
            // Focused/Active state - darker primary
            classes.Add(_themeBorderDarkColor);
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
            classes.Add(_themeBorderLightColor);
            classes.Add(_themeBorderHoverColor);
        }

        return string.Join(" ", classes);
    }

    protected string BuildLegendClass()
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

    protected string BuildRequiredAsteriskClass()
    {
        return $"ml-0.5 {_textAlertColor}";
    }

    protected string GetStartIconClass()
    {
        var classes = new List<string>
        {
            $"absolute flex {GetIconTopOffset()} -translate-y-1/2 left-3 pointer-events-none transition-colors duration-200 z-10"
        };

        // Icon color based on state
        if (HasError)
        {
            classes.Add(_textAlertColor);
        }
        else if (_isFocused)
        {
            classes.Add(_themeTextDarkColor);
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
            classes.Add(_themeTextLightColor);
        }

        return string.Join(" ", classes);
    }

    protected string GetEndIconClass()
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
                classes.Add(_textAlertColor);
            }
            else if (_isFocused)
            {
                classes.Add(_themeTextDarkColor);
            }
            else
            {
                classes.Add(_themeTextLightColor);
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

    protected string GetEndIconLoadingClass()
    {
        var classes = new List<string>
        {
            $"absolute {GetIconTopOffset()} -translate-y-1/2 right-[8px] z-10",
            "flex items-center justify-center"
        };

        // Loading spinner color based on state
        if (HasError)
        {
            classes.Add(_textAlertColor);
        }
        else if (_isFocused)
        {
            classes.Add(_themeTextDarkColor);
        }
        else
        {
            classes.Add(_themeTextLightColor);
        }

        return string.Join(" ", classes);
    }

    protected string GetLoadingSpinnerSize()
    {
        var size = ElementSize switch
        {
            ElementSize.Small => "width: 16px; height: 16px;",
            ElementSize.Large => "width: 20px; height: 20px;",
            _ => "width: 18px; height: 18px;"
        };

        return size;
    }

    protected string GetIconTopOffset()
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

    #region Accessibility Helpers

    protected string GetAriaDescribedByString()
    {
        return !string.IsNullOrEmpty(HelperText) ? $"{Id}-helper" : string.Empty;
    }

    protected string GetAreaInvalidValue()
    {
        return HasError.ToString().ToLowerInvariant();
    }

    protected string? GetTitle()
    {
        return !string.IsNullOrEmpty(Title) ? Title : HelperText;
    }

    #endregion

    #region Theming

    protected void SetThemeColors()
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

        _themeTextLightColor = $"text-(--theme-{colorName}-eight)";
        _themeTextDarkColor = $"text-(--theme-{colorName}-nine)";
        _themeBorderLightColor = $"border-(--theme-{colorName}-eight)";
        _themeBorderDarkColor = $"border-(--theme-{colorName}-nine)";
        _themeBorderHoverColor = $"hover:border-(--theme-{colorName}-nine)";
    }

    #endregion

    #region Private Helpers

    protected void DetachValidationStateChangedListener()
    {
        if (_previousEditContext != null)
        {
            _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }

    #endregion
}