using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleTextAreaModel
{
    [Required]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
    [Display(Name = "Description", Description = "Describe the item in detail")]
    public string? Description { get; set; }

    [Required]
    [StringLength(280, ErrorMessage = "Message cannot exceed 280 characters")]
    [Display(Name = "Message", Description = "Share your thoughts")]
    public string? Message { get; set; }

    [StringLength(1000)]
    [Display(Name = "Biography", Description = "Tell us about yourself")]
    public string? Biography { get; set; }

    [Display(Name = "Comments", Description = "Optional comments")]
    public string? Comments { get; set; } = "This field is read-only and shows pre-filled content that cannot be edited.";

    [Display(Name = "Internal Notes", Description = "Notes are disabled for this record")]
    public string? InternalNotes { get; set; } = "Disabled textareas cannot be focused or edited.";

    [Display(Name = "Feedback", Description = "How can we improve?")]
    public string? Feedback { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 20, ErrorMessage = "Review must be between 20 and 2000 characters")]
    [Display(Name = "Product Review", Description = "Write a detailed product review")]
    public string? Review { get; set; }

    [Display(Name = "Address", Description = "Full mailing address")]
    public string? Address { get; set; }

    [Display(Name = "Notes", Description = "Additional notes")]
    public string? Notes { get; set; }

    [Display(Name = "Summary", Description = "Brief summary")]
    public string? Summary { get; set; }
}