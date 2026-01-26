using System.ComponentModel;

namespace Maviray.Blazor.Components.Core.Enums;

public enum ButtonVariant
{
    /// <summary>
    /// The component has no drop shadow, background or border.
    /// </summary>
    [Description("text")]
    Text,

    /// <summary>
    /// The component interior is filled with a solid color.
    /// </summary>
    [Description("filled")]
    Filled,

    /// <summary>
    /// The component has an outline around the edge.
    /// </summary>
    [Description("outlined")]
    Outlined
}