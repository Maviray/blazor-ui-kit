using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleFormModel
{
    [Required]
    [StringLength(5, ErrorMessage = "Must not be longer than 5 characters.")]
    [Display(Name = "Required Limited String Small", Description = "Please enter whatever comes to mind.")]
    public string? RequiredLimitedStringSmall { get; set; }

    [Required]
    [StringLength(5, ErrorMessage = "Must not be longer than 5 characters.")]
    [Display(Name = "Required Limited String Regular", Description = "This is provided as part of Display attribute.")]
    public string? RequiredLimitedStringRegular { get; set; }

    [Required]
    [StringLength(5, ErrorMessage = "Must not be longer than 5 characters.")]
    [Display(Name = "Required Limited String Large", Description = "This is provided as part of Display attribute.")]
    public string? RequiredLimitedStringLarge { get; set; }
    
    [Display(Name = "Disabled String")]
    public string? DisabledString { get; set; } = "this string is disabled";

    [Display(Name = "Readonly String")]
    public string? ReadonlyString { get; set; } = "this string is readonly";

    public string? UndecoratedString { get; set; }
}