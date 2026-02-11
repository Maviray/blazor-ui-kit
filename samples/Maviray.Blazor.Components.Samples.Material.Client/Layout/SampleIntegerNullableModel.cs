using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Layout;

public class SampleIntegerNullableModel
{
    [Required]
    [Range(1, 100, ErrorMessage = "This number must be between 1 and 100")]
    [Display(Name = "Integer One (Nullable)", Description = "Required nullable integer between 1 and 100.")]
    public int? NumberOne { get; set; }

    [Required]
    [Range(2, 200, ErrorMessage = "This number must be between 2 and 200")]
    [Display(Name = "Integer Two (Nullable)", Description = "Required nullable integer between 2 and 200.")]
    public int? NumberTwo { get; set; }

    [Required]
    [Display(Name = "Integer Three (Nullable)", Description = "Required nullable integer with no range constraint.")]
    public int? NumberThree { get; set; }

    [Display(Name = "Integer Four (Nullable)", Description = "Optional nullable integer - can be null.")]
    public int? NumberFour { get; set; } = 34;

    [Display(Name = "Integer Five (Nullable)", Description = "Optional nullable integer with default value.")]
    public int? NumberFive { get; set; } = 147;

    [Display(Name = "Integer Six (Nullable)", Description = "Optional nullable integer - starts null.")]
    public int? NumberSix { get; set; }

    [Display(Name = "Integer Seven (Nullable)", Description = "Optional nullable integer - starts null.")]
    public int? NumberSeven { get; set; }

    [Display(Name = "FixedPoint (Nullable)", Description = "Nullable integer with fixed point formatting.")]
    public int? FixedPoint { get; set; }

    [Display(Name = "Currency (Nullable)", Description = "Nullable integer with currency formatting.")]
    public int? Currency { get; set; }

    [Display(Name = "Percentage (Nullable)", Description = "Nullable integer with percentage formatting.")]
    public int? Percentage { get; set; }

    [Display(Name = "General (Nullable)", Description = "Nullable integer with general formatting.")]
    public int? General { get; set; }

    [Display(Name = "Zip Code (Nullable)", Description = "Nullable integer for ZIP code.")]
    public int? ZipCode { get; set; }

    [Display(Name = "Separator (Nullable)", Description = "Nullable integer with thousand separators.")]
    public int? Separator { get; set; }

    [Display(Name = "Age (Nullable)", Description = "Optional age field that can be left empty.")]
    [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
    public int? Age { get; set; }

    [Display(Name = "Quantity (Nullable)", Description = "Optional quantity that defaults to null.")]
    public int? Quantity { get; set; }
}