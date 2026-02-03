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

    [Display(Name = "Disabled String")]
    public string? StringThirteen { get; set; } 

    [Display(Name = "Readonly String")]
    public string? StringFourteen { get; set; }

    [Display(Name = "Small Size")]
    public string? StringFifteen { get; set; }

    [Display(Name = "Regular Size")]
    public string? StringSixteen { get; set; }

    [Display(Name = "Large Size")]
    public string? StringSeventeen { get; set; }


    [Required]
    [Display(Name = "Default")]
    public string? Default { get; set; }

    [Required]
    [Display(Name = "Primary")]
    public string? Primary { get; set; }

    [Required]
    [Display(Name = "Secondary")]
    public string? Secondary { get; set; }

    [Required]
    [Display(Name = "Success")]
    public string? Success { get; set; }

    [Required]
    [Display(Name = "Danger")]
    public string? Danger { get; set; }

    [Required]
    [Display(Name = "Warning")]
    public string? Warning { get; set; }

    [Required]
    [Display(Name = "Info")]
    public string? Info { get; set; }

    [Required]
    [Display(Name = "Dark")]
    public string? Dark { get; set; }

    [Required]
    [Display(Name = "Light")]
    public string? Light { get; set; }
}