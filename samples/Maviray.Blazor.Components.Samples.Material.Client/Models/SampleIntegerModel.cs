using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleIntegerModel
{
    [Required]
    [Range(1, 100, ErrorMessage = "This number must be between 1 and 100")]
    [Display(Name = "Integer One", Description = "Custom description as part of Display attribute.")]
    public int NumberOne { get; set; }

    [Required]
    [Range(2, 200, ErrorMessage = "This number must be between 2 and 200")]
    [Display(Name = "Integer Two", Description = "Custom description as part of Display attribute.")]
    public int NumberTwo { get; set; }

    [Required]
    [Display(Name = "Integer Three", Description = "Custom description as part of Display attribute.")]
    public int NumberThree { get; set; }

    [Display(Name = "Integer Four", Description = "Custom description as part of Display attribute.")]
    public int NumberFour { get; set; } = 34;

    [Display(Name = "Integer Five", Description = "Custom description as part of Display attribute.")]
    public int NumberFive { get; set; } = 147;

    [Display(Name = "Integer Six", Description = "Custom description as part of Display attribute.")]
    public int NumberSix { get; set; }

    [Display(Name = "Integer Seven", Description = "Custom description as part of Display attribute.")]
    public int NumberSeven { get; set; }

    [Display(Name = "FixedPoint")]
    public int FixedPoint { get; set; }

    [Display(Name = "Currency")]
    public int Currency { get; set; }

    [Display(Name = "Percentage")]
    public int Percentage { get; set; }

    [Display(Name = "General")]
    public int General { get; set; }

    [Display(Name = "Zip Code")]
    public int ZipCode { get; set; }

    [Display(Name = "Separator")]
    public int Separator { get; set; }
}