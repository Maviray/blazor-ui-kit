using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleStringNullableModel
{
    [Display(Name = "Nick Name", Description = "Your preferred nickname")]
    [MaxLength(50)]
    public string? NickName { get; set; }

    [Display(Name = "Middle Name")]
    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [Display(Name = "Suffix", Prompt = "e.g., Jr., Sr., III")]
    [MaxLength(20)]
    public string? Suffix { get; set; }

    [Display(Name = "Previous Employer")]
    [MaxLength(200)]
    public string? PreviousEmployer { get; set; } = "Acme Corporation";

    [Display(Name = "LinkedIn Profile")]
    [DataType(DataType.Url)]
    public string? LinkedInProfile { get; set; } = "https://linkedin.com/in/example";

    [Display(Name = "Secondary Address")]
    [MaxLength(300)]
    public string? SecondaryAddress { get; set; }

    [Display(Name = "State")]
    [MaxLength(50)]
    public string? State { get; set; }

    [Display(Name = "Security Code")]
    [DataType(DataType.Password)]
    [MaxLength(20)]
    public string? SecurityCode { get; set; }

    [Display(Name = "PIN")]
    [DataType(DataType.Password)]
    [StringLength(6, MinimumLength = 4)]
    public string? Pin { get; set; }

    [Display(Name = "Alternative Email")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string? AlternativeEmail { get; set; }

    [Display(Name = "Fax Number")]
    [DataType(DataType.PhoneNumber)]
    [Phone]
    public string? FaxNumber { get; set; }

    [Display(Name = "Blog URL")]
    [DataType(DataType.Url)]
    [Url]
    public string? BlogUrl { get; set; }

    [Display(Name = "Search Keywords")]
    public string? SearchKeywords { get; set; }

    [Display(Name = "Work Email")]
    [DataType(DataType.EmailAddress)]
    public string? WorkEmail { get; set; }

    [Display(Name = "Home Phone")]
    [DataType(DataType.PhoneNumber)]
    public string? HomePhone { get; set; }

    [Display(Name = "GitHub Profile")]
    [DataType(DataType.Url)]
    [Url]
    public string? GitHubProfile { get; set; }

    [Display(Name = "Skills", Description = "Comma-separated list of skills")]
    [MaxLength(500)]
    public string? Skills { get; set; }

    [Display(Name = "Additional Information")]
    [MaxLength(1000)]
    public string? AdditionalInfo { get; set; }

    [Display(Name = "Twitter Handle")]
    [MaxLength(15)]
    public string? TwitterHandle { get; set; }

    [Display(Name = "Optional Field 1")]
    public string? OptionalField1 { get; set; }

    [Display(Name = "Optional Field 2")]
    public string? OptionalField2 { get; set; } = "Initial Value";

    [Display(Name = "Dark Scheme")]
    [MaxLength(1000)]
    public string? AdditionalInfoDark { get; set; }

    [Display(Name = "Light Scheme")]
    [MaxLength(1000)]
    public string? AdditionalInfoLight { get; set; }
}