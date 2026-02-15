using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Maviray.Blazor.Components.Material.Components.Inputs;

/// <summary>
/// Abstract base class for date input components (both nullable and non-nullable).
/// Contains all shared logic for date parsing, formatting, and calendar picker functionality.
/// </summary>
/// <typeparam name="TValue">Either DateTime or DateTime?</typeparam>
/// <typeparam name="TComponent">Deriving Type</typeparam>
public abstract class MaviInputDateBase<TValue, TComponent> : MaviInputBase<TValue>, IAsyncDisposable where TComponent : MaviInputDateBase<TValue, TComponent>
{
    protected string? _rawInputValue;
    protected bool _isCalendarOpen = false;
    protected ElementReference _containerRef;
    protected IJSObjectReference? _jsModule;
    protected DotNetObjectReference<TComponent>? _dotNetRef;

    #region Date-Specific Parameters

    /// <summary>
    /// Type of date input control to render.
    /// </summary>
    [Parameter] public DateInputType DateInputType { get; set; } = DateInputType.Date;

    /// <summary>
    /// Minimum allowed date value.
    /// </summary>
    [Parameter] public DateTime? Min { get; set; }

    /// <summary>
    /// Maximum allowed date value.
    /// </summary>
    [Parameter] public DateTime? Max { get; set; }

    /// <summary>
    /// Format string for displaying the date.
    /// Common formats:
    /// - "dd/MM/yyyy" (European: 25/12/2024)
    /// - "MM/dd/yyyy" (US: 12/25/2024)
    /// - "dd.MM.yyyy" (German: 25.12.2024)
    /// - "yyyy-MM-dd" (ISO: 2024-12-25)
    /// </summary>
    [Parameter] public string? DisplayFormat { get; set; }

    /// <summary>
    /// Culture info used for parsing and formatting date values. Defaults to current UI culture.
    /// </summary>
    [Parameter] public CultureInfo? FormatCulture { get; set; }

    /// <summary>
    /// When true, shows a custom calendar picker instead of native browser input.
    /// Default is true.
    /// </summary>
    [Parameter] public bool UseCalendarPicker { get; set; } = true;

    [Inject] private IJSRuntime? JsRuntime { get; set; }

    #endregion

    #region Protected Properties

    protected CultureInfo CurrentCulture => FormatCulture ?? CultureInfo.CurrentUICulture;

    #endregion
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && UseCalendarPicker)
        {
            if (JsRuntime == null)
            {
                throw new InvalidOperationException("IJSRuntime is not available. Ensure it is properly injected.");
            }

            _jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Maviray.Blazor.Components.Material/js/maviDatePickerJsInterop.js");
            await _jsModule.InvokeVoidAsync("initializeDatePicker", _containerRef, _dotNetRef);
        }
    }
    #region Lifecycle Methods



    [JSInvokable]
    public void CloseCalendar()
    {
        _isCalendarOpen = false;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("disposeDatePicker", _containerRef);
            await _jsModule.DisposeAsync();
        }

        _dotNetRef?.Dispose();
    }

    #endregion

    #region Input Handling

    protected override Task HandleInput(ChangeEventArgs e)
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        // Don't update value for disabled/readonly fields
        if (Disabled || Readonly)
            return Task.CompletedTask;

        var inputValue = e.Value?.ToString();
        _rawInputValue = inputValue;

        // Try to parse and update the current value
        if (TryParseValueFromString(inputValue, out var parsedValue, out _))
        {
            CurrentValue = parsedValue;
        }

        return Task.CompletedTask;
    }

    protected void HandleInputClick()
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        if (UseCalendarPicker && !Disabled && !Readonly)
        {
            _isCalendarOpen = !_isCalendarOpen;
        }
    }

    protected void ToggleCalendar()
    {
        if (!Disabled)
        {
            _isCalendarOpen = !_isCalendarOpen;
        }
    }

    protected override void OnBlur()
    {
        if (ComponentOptions is { EnableLifecycleLogging: true })
        {
            Logger?.LogDebugLifeCycle(ComponentOptions, Id, GetType());
        }

        base.OnBlur();
        // Clear raw input value on blur - will show formatted value
        _rawInputValue = null;
    }

    #endregion

    #region Attribute Inference

    protected override void InferTypeSpecificAttributes(PropertyInfo propertyInfo)
    {
        // Infer Min date if available
        if (!Min.HasValue)
        {
            var minDateAttr = propertyInfo.GetCustomAttribute<RangeAttribute>();
            if (minDateAttr?.Minimum is DateTime minDateTime)
            {
                Min = minDateTime;
            }
        }

        // Infer Max date if available
        if (!Max.HasValue)
        {
            var maxDateAttr = propertyInfo.GetCustomAttribute<RangeAttribute>();
            if (maxDateAttr?.Maximum is DateTime maxDateTime)
            {
                Max = maxDateTime;
            }
        }

        // Infer DateInputType from DataType attribute
        var dataTypeAttr = propertyInfo.GetCustomAttribute<DataTypeAttribute>();
        if (dataTypeAttr != null)
        {
            DateInputType = dataTypeAttr.DataType switch
            {
                DataType.Date => DateInputType.Date,
                DataType.DateTime => DateInputType.DateTime,
                DataType.Time => DateInputType.Time,
                _ => DateInputType.Date
            };
        }

        // Add helper text for date range if not set
        if (Min.HasValue && Max.HasValue)
        {
            var rangeText = $"Date must be between {Min.Value:d} and {Max.Value:d}";
            if (string.IsNullOrEmpty(HelperText))
            {
                HelperText = rangeText;
            }
            else if (!HelperText.Contains("between"))
            {
                HelperText = $"{HelperText}. {rangeText}";
            }
        }
        else if (Min.HasValue)
        {
            var minText = $"Date must be on or after {Min.Value:d}";
            if (string.IsNullOrEmpty(HelperText))
            {
                HelperText = minText;
            }
        }
        else if (Max.HasValue)
        {
            var maxText = $"Date must be on or before {Max.Value:d}";
            if (string.IsNullOrEmpty(HelperText))
            {
                HelperText = maxText;
            }
        }
    }

    #endregion

    #region Date Formatting and Parsing

    /// <summary>
    /// Gets the display format based on DateInputType and DisplayFormat parameter.
    /// </summary>
    protected string GetDisplayFormat()
    {
        if (!string.IsNullOrEmpty(DisplayFormat))
            return DisplayFormat;

        return DateInputType switch
        {
            DateInputType.Date => "d", // Short date pattern from culture
            DateInputType.DateTime => "g", // Short date/time pattern
            DateInputType.Time => "t", // Short time pattern
            DateInputType.Month => "Y", // Year month pattern
            DateInputType.Week => "yyyy-'W'ww",
            _ => "d"
        };
    }

    /// <summary>
    /// Gets all possible parse formats for the current DateInputType.
    /// </summary>
    protected string[] GetParseFormats()
    {
        // If custom DisplayFormat is set, prioritize it
        if (!string.IsNullOrEmpty(DisplayFormat))
        {
            var formats = new List<string> { DisplayFormat };

            // Add common variations and fallback formats
            formats.AddRange(GetCommonFormatsForType());

            return formats.ToArray();
        }

        return GetCommonFormatsForType();
    }

    /// <summary>
    /// Gets common parse formats based on the DateInputType
    /// </summary>
    protected string[] GetCommonFormatsForType()
    {
        return DateInputType switch
        {
            DateInputType.Date =>
            [
                "yyyy-MM-dd",           // ISO 8601
                "dd/MM/yyyy",           // Common format
                "MM/dd/yyyy",           // US format
                "dd.MM.yyyy",           // European format
                "d/M/yyyy",             // Short format
                "d.M.yyyy",             // Short European
                "M/d/yyyy",             // Short US
                "yyyy/MM/dd",           // Asian format
                "dd-MM-yyyy",           // Dashed format
                "MM-dd-yyyy",           // Dashed US
                "yyyyMMdd",             // Compact format
                "d",                    // Culture short date
                "D"                     // Culture long date
            ],
            DateInputType.DateTime =>
            [
                "yyyy-MM-ddTHH:mm",     // ISO 8601
                "yyyy-MM-ddTHH:mm:ss",  // ISO 8601 with seconds
                "yyyy-MM-dd HH:mm",     // Space separated
                "yyyy-MM-dd HH:mm:ss",  // Space separated with seconds
                "dd/MM/yyyy HH:mm",
                "MM/dd/yyyy HH:mm",
                "dd.MM.yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "g",                    // Culture general date/time
                "G"                     // Culture general date/time with seconds
            ],
            DateInputType.Time =>
            [
                "HH:mm",                // 24-hour
                "HH:mm:ss",             // 24-hour with seconds
                "h:mm tt",              // 12-hour with AM/PM
                "h:mm:ss tt",           // 12-hour with seconds
                "hh:mm tt",
                "t",                    // Culture short time
                "T"                     // Culture long time
            ],
            DateInputType.Month =>
            [
                "yyyy-MM",              // ISO
                "MM/yyyy",
                "MM-yyyy",
                "yyyy/MM",
                "Y"                     // Culture year month
            ],
            DateInputType.Week =>
            [
                "yyyy-'W'ww",           // ISO week format
                "yyyy-Www"
            ],
            _ => ["d"]
        };
    }

    #endregion

    #region Abstract Methods - To be implemented by derived classes

    /// <summary>
    /// Parses the input string into the appropriate date type (DateTime or DateTime?)
    /// </summary>
    protected abstract override bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out TValue result,
        [NotNullWhen(false)] out string? validationErrorMessage);

    /// <summary>
    /// Gets the display value for the input field
    /// </summary>
    protected abstract string? GetDisplayValue();

    /// <summary>
    /// Handles change events (blur)
    /// </summary>
    protected abstract override Task HandleChange(ChangeEventArgs e);

    #endregion
}