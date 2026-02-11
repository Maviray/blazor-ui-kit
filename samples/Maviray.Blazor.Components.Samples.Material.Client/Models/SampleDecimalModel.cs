using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleDecimalModel
{
    [Required]
    [Range(typeof(decimal), "1.0", "100.0", ErrorMessage = "This number must be between 1.0 and 100.0")]
    [Display(Name = "Decimal One", Description = "Custom description as part of Display attribute.")]
    public decimal NumberOne { get; set; }

    [Required]
    [Range(typeof(decimal), "2.5", "200.5", ErrorMessage = "This number must be between 2.5 and 200.5")]
    [Display(Name = "Decimal Two", Description = "Custom description as part of Display attribute.")]
    public decimal NumberTwo { get; set; }

    [Required]
    [Display(Name = "Decimal Three", Description = "Custom description as part of Display attribute.")]
    public decimal NumberThree { get; set; }

    [Display(Name = "Decimal Four", Description = "Custom description as part of Display attribute.")]
    public decimal NumberFour { get; set; } = 34.56m;

    [Display(Name = "Decimal Five", Description = "Custom description as part of Display attribute.")]
    public decimal NumberFive { get; set; } = 147.89m;

    [Display(Name = "Decimal Six", Description = "Custom description as part of Display attribute.")]
    public decimal NumberSix { get; set; }

    [Display(Name = "Decimal Seven", Description = "Custom description as part of Display attribute.")]
    public decimal NumberSeven { get; set; }

    [Display(Name = "Fixed Point 2 Decimals")]
    public decimal FixedPoint2 { get; set; } = 1234.56m;

    [Display(Name = "Fixed Point 4 Decimals")]
    public decimal FixedPoint4 { get; set; } = 1234.5678m;

    [Display(Name = "Currency")]
    public decimal Currency { get; set; } = 1234.56m;

    [Display(Name = "Percentage")]
    public decimal Percentage { get; set; } = 0.1234m;

    [Display(Name = "General Format")]
    public decimal General { get; set; } = 12345.6789m;

    [Display(Name = "Thousand Separators")]
    public decimal Separator { get; set; } = 1234567.89m;

    [Display(Name = "Decimal Places (2)")]
    public decimal DecimalPlaces2 { get; set; } = 123.456789m;

    [Display(Name = "Decimal Places (4)")]
    public decimal DecimalPlaces4 { get; set; } = 123.456789m;

    [Display(Name = "Price")]
    public decimal Price { get; set; } = 99.99m;

    [Display(Name = "Interest Rate")]
    public decimal InterestRate { get; set; } = 5.75m;

    [Display(Name = "Account Balance")]
    public decimal AccountBalance { get; set; } = 15432.89m;

    [Display(Name = "Tax Amount")]
    public decimal TaxAmount { get; set; } = 1250.00m;

    [Display(Name = "Invoice Total")]
    public decimal InvoiceTotal { get; set; } = 9875.50m;
}