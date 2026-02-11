using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleDoubleModel
{
    [Required]
    [Range(1.0, 100.0, ErrorMessage = "This number must be between 1.0 and 100.0")]
    [Display(Name = "Double One", Description = "Custom description as part of Display attribute.")]
    public double NumberOne { get; set; }

    [Required]
    [Range(2.5, 200.5, ErrorMessage = "This number must be between 2.5 and 200.5")]
    [Display(Name = "Double Two", Description = "Custom description as part of Display attribute.")]
    public double NumberTwo { get; set; }

    [Required]
    [Display(Name = "Double Three", Description = "Custom description as part of Display attribute.")]
    public double NumberThree { get; set; }

    [Display(Name = "Double Four", Description = "Custom description as part of Display attribute.")]
    public double NumberFour { get; set; } = 34.56;

    [Display(Name = "Double Five", Description = "Custom description as part of Display attribute.")]
    public double NumberFive { get; set; } = 147.89;

    [Display(Name = "Double Six", Description = "Custom description as part of Display attribute.")]
    public double NumberSix { get; set; }

    [Display(Name = "Double Seven", Description = "Custom description as part of Display attribute.")]
    public double NumberSeven { get; set; }

    [Display(Name = "Fixed Point 2 Decimals")]
    public double FixedPoint2 { get; set; } = 1234.56;

    [Display(Name = "Fixed Point 4 Decimals")]
    public double FixedPoint4 { get; set; } = 1234.5678;

    [Display(Name = "Currency")]
    public double Currency { get; set; } = 1234.56;

    [Display(Name = "Percentage")]
    public double Percentage { get; set; } = 0.1234;

    [Display(Name = "Scientific Notation")]
    public double Scientific { get; set; } = 1234567.89;

    [Display(Name = "General Format")]
    public double General { get; set; } = 12345.6789;

    [Display(Name = "Thousand Separators")]
    public double Separator { get; set; } = 1234567.89;

    [Display(Name = "Decimal Places (2)")]
    public double DecimalPlaces2 { get; set; } = 123.456789;

    [Display(Name = "Decimal Places (4)")]
    public double DecimalPlaces4 { get; set; } = 123.456789;

    [Display(Name = "Price")]
    public double Price { get; set; } = 99.99;

    [Display(Name = "Temperature")]
    public double Temperature { get; set; } = 98.6;

    [Display(Name = "Weight (kg)")]
    public double Weight { get; set; } = 75.5;
}