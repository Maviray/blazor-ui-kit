using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Components.Inputs;

public abstract class MaviNumericInputBase<TValue> : MaviMaterialInputBase<TValue>
{
    protected string? RawInputValue;

    /// <summary>
    ///     Culture info used for parsing and formatting numeric values. Defaults to current UI culture.
    /// </summary>
    [Parameter]
    public CultureInfo? FormatCulture { get; set; }

    /// <summary>
    ///     Format string for displaying the value (e.g., "N2", "F4", "C", etc.).
    /// </summary>
    [Parameter]
    public string? FormatMask { get; set; }

    /// <summary>
    ///     When true, shows formatted value even when focused (editing).
    ///     Default is false - shows raw number when focused for easier editing.
    /// </summary>
    [Parameter]
    public bool ShowFormattedWhileFocused { get; set; }

    protected CultureInfo CurrentCulture => FormatCulture ?? CultureInfo.CurrentUICulture;

    protected override void OnBlur()
    {
        base.OnBlur();

        // Clear raw input value on blur - will show formatted value
        RawInputValue = null;
    }

    #region Abstract Methods (Must be implemented by derived classes)

    /// <summary>
    ///     Gets the value to display in the input field.
    /// </summary>
    protected abstract string? GetDisplayValue();

    /// <summary>
    ///     Gets the HTML input type attribute value.
    /// </summary>
    protected abstract string GetInputType();

    /// <summary>
    ///     Gets the HTML inputmode attribute value for mobile keyboards.
    /// </summary>
    protected abstract string GetInputMode();

    /// <summary>
    ///     Gets the min attribute value as a string.
    /// </summary>
    protected abstract string? GetMinAttribute();

    /// <summary>
    ///     Gets the max attribute value as a string.
    /// </summary>
    protected abstract string? GetMaxAttribute();

    /// <summary>
    ///     Gets the step attribute value as a string.
    /// </summary>
    protected abstract string? GetStepAttribute();

    #endregion
}