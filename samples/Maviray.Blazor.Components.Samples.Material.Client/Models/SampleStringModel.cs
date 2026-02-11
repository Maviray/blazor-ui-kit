using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleStringModel
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [Display(Name = "Username", Description = "Enter your username", Prompt = "Choose a unique username")]
    public string? Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Full name must be between 5 and 100 characters")]
    [Display(Name = "Full Name", Description = "Enter your full legal name")]
    public string? FullName { get; set; }

    [Required]
    [StringLength(200, ErrorMessage = "Bio cannot exceed 200 characters")]
    [Display(Name = "Biography", Description = "Tell us about yourself")]
    public string? Biography { get; set; }

    [Display(Name = "Company Name", Description = "Your company or organization")]
    public string? CompanyName { get; set; } = "Acme Corp";

    [Display(Name = "Job Title", Description = "Your current position")]
    public string? JobTitle { get; set; } = "Software Engineer";

    [Display(Name = "Address", Description = "Your street address")]
    public string? Address { get; set; }

    [Display(Name = "City", Description = "City of residence")]
    public string? City { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [Display(Name = "Password", Description = "Create a strong password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Description = "Re-enter your password")]
    public string? ConfirmPassword { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email Address", Description = "Your primary email")]
    public string? Email { get; set; }

    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [Display(Name = "Secondary Email", Description = "Optional backup email")]
    public string? SecondaryEmail { get; set; }

    [Required]
    [DataType(DataType.PhoneNumber)]
    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone Number", Description = "Your contact number")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.PhoneNumber)]
    [Phone]
    [Display(Name = "Mobile Number", Description = "Optional mobile number")]
    public string? MobileNumber { get; set; }

    [DataType(DataType.Url)]
    [Url(ErrorMessage = "Invalid URL format")]
    [Display(Name = "Website URL", Description = "Your personal or company website")]
    public string? WebsiteUrl { get; set; }

    [DataType(DataType.Url)]
    [Url]
    [Display(Name = "Portfolio URL", Description = "Link to your portfolio")]
    public string? PortfolioUrl { get; set; }

    [Display(Name = "Search Query", Description = "Enter search terms")]
    public string? SearchQuery { get; set; }

    [StringLength(500, MinimumLength = 10, ErrorMessage = "Notes must be between 10 and 500 characters")]
    [Display(Name = "Notes", Description = "Additional notes or comments")]
    public string? Notes { get; set; }

    [Display(Name = "Tags", Description = "Comma-separated tags")]
    public string? Tags { get; set; }

    [Display(Name = "Department", Description = "Your department")]
    public string? Department { get; set; }
}