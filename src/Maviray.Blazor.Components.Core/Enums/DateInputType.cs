namespace Maviray.Blazor.Components.Core.Enums;

/// <summary>
///     Defines the types of date/time input controls.
///     Maps to HTML5 input types for native date pickers.
/// </summary>
public enum DateInputType
{
    /// <summary>
    ///     Date input (year, month, day). HTML5 type="date"
    ///     Format: yyyy-MM-dd
    /// </summary>
    Date,

    /// <summary>
    ///     Date and time input (no timezone). HTML5 type="datetime-local"
    ///     Format: yyyy-MM-ddTHH:mm
    /// </summary>
    DateTime,

    /// <summary>
    ///     Time input (hours and minutes). HTML5 type="time"
    ///     Format: HH:mm
    /// </summary>
    Time,

    /// <summary>
    ///     Month and year input. HTML5 type="month"
    ///     Format: yyyy-MM
    /// </summary>
    Month,

    /// <summary>
    ///     Week and year input. HTML5 type="week"
    ///     Format: yyyy-Www
    /// </summary>
    Week
}