using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleMultiSelectModel
{
    [Display(Name = "Countries")]
    public List<string>? Countries { get; set; }

    [Display(Name = "States")]
    public List<string>? States { get; set; }

    [Display(Name = "Cities")]
    public List<string>? Cities { get; set; }

    [Display(Name = "Departments")]
    public List<string>? Departments { get; set; }

    [Display(Name = "Job Roles")]
    public List<string>? JobRoles { get; set; }

    [Display(Name = "Languages")]
    public List<string>? Languages { get; set; }

    [Display(Name = "Skill Levels")]
    public List<SkillLevel>? SkillLevels { get; set; }

    [Display(Name = "Statuses")]
    public List<Status>? Statuses { get; set; }

    [Display(Name = "Categories")]
    public List<Category>? Categories { get; set; }

    [Display(Name = "Country Codes")]
    public List<string>? CountryCodes { get; set; }

    [Display(Name = "Department Names")]
    public List<string>? DepartmentNames { get; set; }

    [Display(Name = "Currencies")]
    public List<string>? Currencies { get; set; }

    [Display(Name = "Timezones")]
    public List<string>? Timezones { get; set; }

    [Display(Name = "Color Themes")]
    public List<string>? ColorThemes { get; set; }

    [Display(Name = "Skills")]
    public List<string>? Skills { get; set; }

    [Display(Name = "Priorities")]
    public List<Priority>? Priorities { get; set; }
}