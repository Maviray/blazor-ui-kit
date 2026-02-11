using System.ComponentModel.DataAnnotations;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleFloatModel
{
    [Required]
    [Range(typeof(float), "-1000", "1000", ErrorMessage = "Value must be between -1000 and 1000")]
    [Display(Name = "Float One", Description = "Custom description as part of Display attribute.")]
    public float NumberOne { get; set; }

    [Required]
    [Range(typeof(float), "0", "360", ErrorMessage = "Value must be between 0 and 360")]
    [Display(Name = "Float Two", Description = "Custom description as part of Display attribute.")]
    public float NumberTwo { get; set; }

    [Required]
    [Display(Name = "Float Three", Description = "Custom description as part of Display attribute.")]
    public float NumberThree { get; set; }

    [Display(Name = "Float Four", Description = "Custom description as part of Display attribute.")]
    public float NumberFour { get; set; } = 12.34f;

    [Display(Name = "Float Five", Description = "Custom description as part of Display attribute.")]
    public float NumberFive { get; set; } = 56.78f;

    [Display(Name = "Float Six", Description = "Custom description as part of Display attribute.")]
    public float NumberSix { get; set; }

    [Display(Name = "Fixed Point 2 Decimals")]
    public float FixedPoint2 { get; set; } = 123.45f;

    [Display(Name = "Fixed Point 4 Decimals")]
    public float FixedPoint4 { get; set; } = 123.4567f;

    [Display(Name = "Scientific Notation")]
    public float Scientific { get; set; } = 1234.56f;

    [Display(Name = "General Format")]
    public float General { get; set; } = 123.456f;

    [Display(Name = "Number with Separators")]
    public float Separator { get; set; } = 1234.56f;

    [Display(Name = "Custom Format")]
    public float Custom { get; set; } = 0.123f;

    [Display(Name = "Decimal Places (2)")]
    public float DecimalPlaces2 { get; set; } = 123.456f;

    [Display(Name = "Decimal Places (3)")]
    public float DecimalPlaces3 { get; set; } = 0.7854f; // π/4

    // 3D Graphics
    [Display(Name = "Position X")]
    [Range(typeof(float), "-1000", "1000")]
    public float PositionX { get; set; } = 10.5f;

    [Display(Name = "Position Y")]
    [Range(typeof(float), "-1000", "1000")]
    public float PositionY { get; set; } = 25.3f;

    [Display(Name = "Position Z")]
    [Range(typeof(float), "-1000", "1000")]
    public float PositionZ { get; set; } = -5.8f;

    // Color Components (0-1 normalized)
    [Display(Name = "Red Channel")]
    [Range(typeof(float), "0", "1")]
    public float ColorRed { get; set; } = 0.85f;

    [Display(Name = "Green Channel")]
    [Range(typeof(float), "0", "1")]
    public float ColorGreen { get; set; } = 0.32f;

    [Display(Name = "Blue Channel")]
    [Range(typeof(float), "0", "1")]
    public float ColorBlue { get; set; } = 0.67f;

    // Physics
    [Display(Name = "Velocity (m/s)")]
    [Range(typeof(float), "-100", "100")]
    public float Velocity { get; set; } = 9.8f;

    [Display(Name = "Rotation Angle (degrees)")]
    [Range(typeof(float), "0", "360")]
    public float RotationAngle { get; set; } = 45.0f;

    [Display(Name = "Scale Factor")]
    [Range(typeof(float), "0.01", "10")]
    public float ScaleFactor { get; set; } = 1.0f;

    // Sensors
    [Display(Name = "Temperature (°C)")]
    [Range(typeof(float), "-50", "150")]
    public float Temperature { get; set; } = 22.5f;

    [Display(Name = "Humidity (%)")]
    [Range(typeof(float), "0", "100")]
    public float Humidity { get; set; } = 65.3f;

    [Display(Name = "Acceleration (m/s²)")]
    [Range(typeof(float), "-50", "50")]
    public float Acceleration { get; set; } = 9.81f; // Earth's gravity

    [Display(Name = "Pressure (kPa)")]
    [Range(typeof(float), "0", "200")]
    public float Pressure { get; set; } = 101.325f; // Standard atmospheric pressure

    [Display(Name = "Audio Frequency (Hz)")]
    [Range(typeof(float), "20", "20000")]
    public float AudioFrequency { get; set; } = 440.0f; // A4 note

    [Display(Name = "Voltage (V)")]
    [Range(typeof(float), "0", "240")]
    public float Voltage { get; set; } = 5.0f; // Common USB voltage
}