using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleFormModel
{
    [Required]
    [StringLength(200, ErrorMessage = "Must not be longer than 200 characters.")]
    [Display(Name = "Required Limited String One", Description = "Custom description as part of Display attribute.")]
    public string? StringOne { get; set; }
    
    [StringLength(200, ErrorMessage = "Must not be longer than 200 characters.")]
    [Display(Name = "Limited String Regular")]
    public string? StringTWo { get; set; }
    
    [Display(Name = "String Large", Description = "Custom description as part of Display attribute.")]
    public string? StringThree { get; set; }
    
    [Display(Name = "Disabled String")]
    public string? StringFour { get; set; } = "this string is disabled";

    [Display(Name = "Readonly String")]
    public string? StringFive { get; set; } = "this string is readonly";

    // undecorated string property
    public string? StringSix { get; set; }

    [Required]
    public string? StringSeven { get; set; }

    [Required]
    public string? StringEight { get; set; }

    public string? StringNine { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(200, ErrorMessage = "Must not be longer than 200 characters.")]
    [Display(Name = "Password", Description = "Required field.")]
    public string? StringTen { get; set; }

    [Required]
    public string? StringEleven { get; set; }

    [StringLength(200, ErrorMessage = "Must not be longer than 200 characters.")]
    [Display(Name = "Required Limited String One", Description = "Custom description as part of Display attribute.")]
    public string? StringTwelve { get; set; }
}