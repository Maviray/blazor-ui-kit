using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleShortModel
{
    [Required]
    [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
    [Display(Name = "Short One", Description = "Custom description as part of Display attribute.")]
    public short NumberOne { get; set; }

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    [Display(Name = "Short Two", Description = "Custom description as part of Display attribute.")]
    public short NumberTwo { get; set; }

    [Required]
    [Display(Name = "Short Three", Description = "Custom description as part of Display attribute.")]
    public short NumberThree { get; set; }

    [Display(Name = "Short Four", Description = "Custom description as part of Display attribute.")]
    public short NumberFour { get; set; } = 42;

    [Display(Name = "Short Five", Description = "Custom description as part of Display attribute.")]
    public short NumberFive { get; set; } = 100;

    [Display(Name = "Short Six", Description = "Custom description as part of Display attribute.")]
    public short NumberSix { get; set; }

    [Display(Name = "Short Seven", Description = "Custom description as part of Display attribute.")]
    public short NumberSeven { get; set; }

    [Display(Name = "Standard")]
    public short Standard { get; set; } = 1234;

    [Display(Name = "Year (Padded)")]
    public short YearPadded { get; set; } = 2024;

    [Display(Name = "With Separators")]
    public short WithSeparators { get; set; } = 12345;

    [Display(Name = "Hexadecimal")]
    public short Hexadecimal { get; set; } = 12345; // 0x3039 in hex

    [Display(Name = "Custom Format")]
    public short Custom { get; set; } = 123;

    [Display(Name = "Age")]
    [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
    public short Age { get; set; } = 25;

    [Display(Name = "Birth Year")]
    [Range(1900, 2024, ErrorMessage = "Birth year must be between 1900 and 2024")]
    public short BirthYear { get; set; } = 1990;

    [Display(Name = "Quantity")]
    [Range(1, 999, ErrorMessage = "Quantity must be between 1 and 999")]
    public short Quantity { get; set; } = 10;

    [Display(Name = "Priority")]
    [Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
    public short Priority { get; set; } = 50;

    [Display(Name = "Temperature (°C)")]
    [Range(-50, 150, ErrorMessage = "Temperature must be between -50°C and 150°C")]
    public short Temperature { get; set; } = 22;

    [Display(Name = "HTTP Status Code")]
    [Range(100, 599, ErrorMessage = "Status code must be between 100 and 599")]
    public short StatusCode { get; set; } = 200;

    [Display(Name = "Percentage")]
    [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
    public short Percentage { get; set; } = 75;

    [Display(Name = "Page Number")]
    [Range(1, 9999, ErrorMessage = "Page number must be between 1 and 9999")]
    public short PageNumber { get; set; } = 1;

    [Display(Name = "Employee Count")]
    [Range(1, 30000, ErrorMessage = "Employee count must be between 1 and 30,000")]
    public short EmployeeCount { get; set; } = 250;
}