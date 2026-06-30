using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleDropdownModel
{
    [Required]
    [Display(Name = "Country", Description = "Select your country")]
    public string? Country { get; set; }

    [Required]
    [Display(Name = "State/Province", Description = "Select your state or province")]
    public string? State { get; set; }

    [Display(Name = "City", Description = "Select your city")]
    public string? City { get; set; }

    [Required]
    [Display(Name = "Department", Description = "Your department")]
    public string? Department { get; set; }

    [Display(Name = "Job Role", Description = "Your current role")]
    public string? JobRole { get; set; }

    [Required]
    [Display(Name = "Experience Level", Description = "Years of experience")]
    public int? ExperienceLevel { get; set; }

    [Display(Name = "Skill Level", Description = "Your expertise level")]
    public SkillLevel SkillLevel { get; set; }

    [Required]
    [Display(Name = "Priority", Description = "Task priority")]
    public Priority Priority { get; set; }

    [Display(Name = "Status", Description = "Current status")]
    public Status Status { get; set; }

    [Display(Name = "Category", Description = "Item category")]
    public Category Category { get; set; }

    [Display(Name = "Size", Description = "Product size")]
    public ProductSize ProductSize { get; set; }

    [Display(Name = "Color Theme", Description = "Preferred color theme")]
    public string? ColorTheme { get; set; }

    [Display(Name = "Language", Description = "Preferred language")]
    public string? Language { get; set; }

    [Display(Name = "Currency", Description = "Preferred currency")]
    public string? Currency { get; set; }

    [Display(Name = "Timezone", Description = "Your timezone")]
    public string? Timezone { get; set; }

    [Display(Name = "Language", Description = "Manual validation demo")]
    public string? ManualState { get; set; }
}

public enum SkillLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

public enum Status
{
    Active,
    Inactive,
    Pending,
    Completed,
    Cancelled
}

public enum Category
{
    Electronics,
    Clothing,
    Food,
    Books,
    Toys,
    Sports
}

public enum ProductSize
{
    XSmall,
    Small,
    Medium,
    Large,
    XLarge,
    XXLarge
}

// Sample data classes
public class Country
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ExperienceOption
{
    public int Years { get; set; }
    public string Label { get; set; } = string.Empty;
}