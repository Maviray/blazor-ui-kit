using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleDateNullableModel
{
    [Display(Name = "Appointment Date", Description = "When would you like your appointment?")]
    public DateTime? DateOne { get; set; } = DateTime.Today;

    [Display(Name = "Event Date")]
    public DateTime? DateTwo { get; set; } = DateTime.Today.AddDays(7);

    [Display(Name = "Meeting Date")]
    public DateTime? DateThree { get; set; } = DateTime.Today.AddDays(14);

    [Display(Name = "Disabled Date")]
    public DateTime? DateFour { get; set; } = DateTime.Today;

    [Display(Name = "Readonly Date")]
    public DateTime? DateFive { get; set; } = DateTime.Today.AddDays(-30);

    [Display(Name = "Editable Date")]
    public DateTime? DateSix { get; set; } = DateTime.Today;

    [Display(Name = "Simple Date")]
    [DataType(DataType.Date)]
    public DateTime? DateSeven { get; set; } = DateTime.Today;

    [Display(Name = "Date and Time")]
    [DataType(DataType.DateTime)]
    public DateTime? DateEight { get; set; } = DateTime.Now;

    [Display(Name = "Time Only")]
    [DataType(DataType.Time)]
    public DateTime? DateNine { get; set; } = DateTime.Now;

    [Display(Name = "Month and Year")]
    public DateTime? DateTen { get; set; } = DateTime.Today;

    [Display(Name = "Week Selection")]
    public DateTime? DateEleven { get; set; } = DateTime.Today;

    [Display(Name = "Future Appointment", Description = "Select a date within the next year")]
    public DateTime? DateTwelve { get; set; } = DateTime.Today.AddDays(30);

    [Display(Name = "Birth Date", Description = "You must be at least 18 years old")]
    public DateTime? DateThirteen { get; set; } = DateTime.Today.AddYears(-25);

    [Display(Name = "US Date Format")]
    public DateTime? DateFourteen { get; set; } = DateTime.Today;

    [Display(Name = "European Date Format")]
    public DateTime? DateFifteen { get; set; } = DateTime.Today;

    [Display(Name = "Clearable Date")]
    public DateTime? DateSixteen { get; set; } = DateTime.Today;

    [Display(Name = "Toggle Date")]
    public DateTime? DateSeventeen { get; set; } = DateTime.Today;

    [Display(Name = "Primary Theme")]
    public DateTime? DateEighteen { get; set; } = DateTime.Today;

    [Display(Name = "Secondary Theme")]
    public DateTime? DateNineteen { get; set; } = DateTime.Today;

    [Display(Name = "Success Theme")]
    public DateTime? DateTwenty { get; set; } = DateTime.Today;

    [Display(Name = "Alert Theme")]
    public DateTime? DateTwentyOne { get; set; } = DateTime.Today;

    [Display(Name = "Optional Date (No Default)")]
    public DateTime? DateTwentyTwo { get; set; } = null;

    [Display(Name = "Optional Future Date")]
    public DateTime? DateTwentyThree { get; set; } = null;

    [Display(Name = "Optional Past Date")]
    public DateTime? DateTwentyFour { get; set; } = null;
}