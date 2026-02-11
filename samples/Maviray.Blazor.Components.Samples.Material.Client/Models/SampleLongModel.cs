using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleLongModel
{
    [Required]
    [Range(typeof(long), "1", "9223372036854775807", ErrorMessage = "This number must be between 1 and 9,223,372,036,854,775,807")]
    [Display(Name = "Long One", Description = "Custom description as part of Display attribute.")]
    public long NumberOne { get; set; }

    [Required]
    [Range(typeof(long), "100", "1000000000", ErrorMessage = "This number must be between 100 and 1,000,000,000")]
    [Display(Name = "Long Two", Description = "Custom description as part of Display attribute.")]
    public long NumberTwo { get; set; }

    [Required]
    [Display(Name = "Long Three", Description = "Custom description as part of Display attribute.")]
    public long NumberThree { get; set; }

    [Display(Name = "Long Four", Description = "Custom description as part of Display attribute.")]
    public long NumberFour { get; set; } = 1234567890L;

    [Display(Name = "Long Five", Description = "Custom description as part of Display attribute.")]
    public long NumberFive { get; set; } = 9876543210L;

    [Display(Name = "Long Six", Description = "Custom description as part of Display attribute.")]
    public long NumberSix { get; set; }

    [Display(Name = "Long Seven", Description = "Custom description as part of Display attribute.")]
    public long NumberSeven { get; set; }

    [Display(Name = "Standard Number")]
    public long Standard { get; set; } = 1234567890L;

    [Display(Name = "With Separators")]
    public long WithSeparators { get; set; } = 1234567890123L;

    [Display(Name = "Hexadecimal")]
    public long Hexadecimal { get; set; } = 1234567890L;

    [Display(Name = "Currency in Cents")]
    public long CurrencyCents { get; set; } = 123456789L; // $1,234,567.89

    [Display(Name = "Custom Format")]
    public long Custom { get; set; } = 1234567890L;

    [Display(Name = "Database ID (BIGINT)")]
    public long DatabaseId { get; set; } = 9876543210123456L;

    [Display(Name = "Unix Timestamp (ms)")]
    public long UnixTimestamp { get; set; } = 1707667200000L; // Feb 11, 2024

    [Display(Name = "File Size (Bytes)")]
    public long FileSize { get; set; } = 5368709120L; // 5 GB in bytes

    [Display(Name = "Population")]
    public long Population { get; set; } = 8000000000L; // 8 billion (world population)

    [Display(Name = "Twitter Snowflake ID")]
    public long TwitterSnowflakeId { get; set; } = 1359634243223228416L;

    [Display(Name = "Distance (millimeters)")]
    public long DistanceMillimeters { get; set; } = 384400000000L; // Earth to Moon in mm

    [Display(Name = "DateTime Ticks")]
    public long DateTimeTicks { get; set; } = 638429280000000000L; // .NET DateTime ticks

    [Display(Name = "Memory Address (64-bit)")]
    public long MemoryAddress { get; set; } = 0x00007FF7B4A10000L;

    [Display(Name = "National Debt (cents)")]
    public long NationalDebtCents { get; set; } = 3400000000000000L; // $34 trillion in cents
}