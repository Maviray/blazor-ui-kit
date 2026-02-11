using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleDoubleNullableModel
{
    [Required]
    [Range(1.0, 100.0, ErrorMessage = "This number must be between 1.0 and 100.0")]
    [Display(Name = "Double One (Nullable)", Description = "Required nullable double between 1.0 and 100.0.")]
    public double? NumberOne { get; set; }

    [Required]
    [Range(2.0, 200.0, ErrorMessage = "This number must be between 2.0 and 200.0")]
    [Display(Name = "Double Two (Nullable)", Description = "Required nullable double between 2.0 and 200.0.")]
    public double? NumberTwo { get; set; }

    [Required]
    [Display(Name = "Double Three (Nullable)", Description = "Required nullable double with no range constraint.")]
    public double? NumberThree { get; set; }

    [Display(Name = "Double Four (Nullable)", Description = "Optional nullable double - can be null.")]
    public double? NumberFour { get; set; } = 34.56;

    [Display(Name = "Double Five (Nullable)", Description = "Optional nullable double with default value.")]
    public double? NumberFive { get; set; } = 147.89;

    [Display(Name = "Double Six (Nullable)", Description = "Optional nullable double - starts null.")]
    public double? NumberSix { get; set; }

    [Display(Name = "Double Seven (Nullable)", Description = "Optional nullable double - starts null.")]
    public double? NumberSeven { get; set; }

    [Display(Name = "FixedPoint (Nullable)", Description = "Nullable double with fixed point formatting.")]
    public double? FixedPoint { get; set; }

    [Display(Name = "Currency (Nullable)", Description = "Nullable double with currency formatting.")]
    public double? Currency { get; set; }

    [Display(Name = "Percentage (Nullable)", Description = "Nullable double with percentage formatting.")]
    public double? Percentage { get; set; }

    [Display(Name = "General (Nullable)", Description = "Nullable double with general formatting.")]
    public double? General { get; set; }

    [Display(Name = "Scientific (Nullable)", Description = "Nullable double with scientific notation.")]
    public double? Scientific { get; set; }

    [Display(Name = "Number (Nullable)", Description = "Nullable double with number formatting.")]
    public double? Number { get; set; }

    [Display(Name = "Price (Nullable)", Description = "Optional price field that can be left empty.")]
    [Range(0.0, 999999.99, ErrorMessage = "Price must be between 0.00 and 999,999.99")]
    public double? Price { get; set; }

    [Display(Name = "Temperature (Nullable)", Description = "Optional temperature measurement.")]
    [Range(-273.15, 1000.0, ErrorMessage = "Temperature must be between -273.15 and 1000.0")]
    public double? Temperature { get; set; }

    [Display(Name = "Weight (Nullable)", Description = "Optional weight in kilograms.")]
    public double? Weight { get; set; }

    [Display(Name = "Discount Rate (Nullable)", Description = "Optional discount percentage.")]
    [Range(0.0, 100.0, ErrorMessage = "Discount must be between 0% and 100%")]
    public double? DiscountRate { get; set; }
}