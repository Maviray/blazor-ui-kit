namespace Maviray.Blazor.Components.Core.Constants;

/// <summary>
///     Provides predefined format masks for numeric input components.
/// </summary>
/// Standard Formats:
/// N or N0 - Number: 1,234
/// C or C0 - Currency: $1,234
/// D5 - Decimal (padded): 00123
/// X - Hexadecimal: 4D2
/// P0 - Percentage: 12%
/// G - General: 1234
/// F2 - Fixed-point: 1234.00
/// Custom Formats:
/// #,##0 - Thousand separators: 1,234
/// 000000 - Zero padding: 001234
/// #,##0.00 - Decimals with separators (limited to integer range)
public static class InputFormatMasks
{
    #region Integer Formats (Safe for InputBase<int>)

    /// <summary>
    ///     General format - most compact representation.
    ///     Example: 1234
    ///     Safe for type="number"
    /// </summary>
    public const string GENERAL = "G";

    /// <summary>
    ///     Fixed-point format with no decimal places.
    ///     Example: 1234
    ///     Safe for type="number"
    /// </summary>
    public const string FIXED_POINT = "F0";

    /// <summary>
    ///     Decimal format padded to 5 digits with leading zeros.
    ///     Example: 00123
    ///     Safe for type="number"
    /// </summary>
    public const string DECIMAL_PADDED5 = "D5";

    /// <summary>
    ///     Decimal format padded to 6 digits with leading zeros.
    ///     Example: 000123
    ///     Safe for type="number"
    /// </summary>
    public const string DECIMAL_PADDED6 = "D6";

    /// <summary>
    ///     Decimal format padded to 8 digits with leading zeros.
    ///     Example: 00000123
    ///     Safe for type="number"
    /// </summary>
    public const string DECIMAL_PADDED8 = "D8";

    /// <summary>
    ///     Custom format with zero padding to 6 digits.
    ///     Example: 001234
    ///     Safe for type="number"
    /// </summary>
    public const string ZERO_PADDED6 = "000000";

    /// <summary>
    ///     Custom format with zero padding to 8 digits.
    ///     Example: 00001234
    ///     Safe for type="number"
    /// </summary>
    public const string ZERO_PADDED8 = "00000000";

    /// <summary>
    ///     Custom format with zero padding to 10 digits.
    ///     Example: 0000001234
    ///     Safe for type="number"
    /// </summary>
    public const string ZERO_PADDED10 = "0000000000";

    /// <summary>
    ///     Custom format for postal/zip code (5 digits).
    ///     Example: 12345
    ///     Safe for type="number"
    /// </summary>
    public const string ZIP_CODE5 = "00000";

    #endregion

    #region Integer Formats with Special Characters (Requires type="text")

    /// <summary>
    ///     Number format with thousand separators and no decimal places.
    ///     Example: 1,234
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string NUMBER = "N0";

    /// <summary>
    ///     Custom format with thousand separators.
    ///     Example: 1,234
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string THOUSAND_SEPARATORS = "#,##0";

    /// <summary>
    ///     Currency format with thousand separators and no decimal places.
    ///     Example: $1,234
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string CURRENCY = "C0";

    /// <summary>
    ///     Custom percentage format - displays the integer value with % symbol.
    ///     Example: Input 4 displays as "4%"
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    ///     Note: Does NOT multiply by 100 (use for integer percentage values)
    /// </summary>
    public const string CUSTOM_PERCENTAGE = "0'%'";

    /// <summary>
    ///     Custom percentage format with 1 decimal place.
    ///     Example: Input 45 displays as "45.0%"
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    ///     Note: Does NOT multiply by 100 (use for integer percentage values)
    /// </summary>
    public const string PERCENTAGE_WITH_ONE_DECIMAL = "0.0'%'";

    /// <summary>
    ///     Custom percentage format with 2 decimal places.
    ///     Example: Input 45 displays as "45.00%"
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    ///     Note: Does NOT multiply by 100 (use for integer percentage values)
    /// </summary>
    public const string PERCENTAGE_WITH_TWO_DECIMALS = "0.00'%'";

    /// <summary>
    ///     Standard .NET percentage format (multiplies by 100).
    ///     Example: For decimal value 0.04, displays as "400%" for integer 4
    ///     WARNING: Not suitable for integer inputs - use CUSTOM_PERCENTAGE instead
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string PERCENTAGE = "P0";

    /// <summary>
    ///     Hexadecimal format (uppercase).
    ///     Example: 4D2
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string HEXADECIMAL = "X";

    /// <summary>
    ///     Hexadecimal format (lowercase).
    ///     Example: 4d2
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string HEXADECIMAL_LOWERCASE = "x";

    /// <summary>
    ///     Hexadecimal format padded to 4 digits (uppercase).
    ///     Example: 04D2
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string HEXADECIMAL_PADDED4 = "X4";

    /// <summary>
    ///     Hexadecimal format padded to 8 digits (uppercase).
    ///     Example: 000004D2
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string HEXADECIMAL_PADDED8 = "X8";

    /// <summary>
    ///     Format for quantities (with thousand separators).
    ///     Example: 1,234
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string QUANTITY = "#,##0";

    /// <summary>
    ///     Format for file sizes in bytes.
    ///     Example: 1,234 bytes
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string BYTES = "#,##0";

    /// <summary>
    ///     Format for kilobytes.
    ///     Example: 1,234 KB
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string KILOBYTES = "#,##0";

    /// <summary>
    ///     Format for megabytes.
    ///     Example: 1,234 MB
    ///     WARNING: Requires type="text" - causes browser error with type="number"
    /// </summary>
    public const string MEGABYTES = "#,##0";

    #endregion

    #region Decimal/Float Formats (For InputBase<decimal>, InputBase<double>, InputBase<float>)

    /// <summary>
    ///     Number format with thousand separators and 2 decimal places.
    ///     Example: 1,234.56
    ///     For use with decimal/double/float types
    /// </summary>
    public const string NUMBER_WITH_DECIMALS = "N2";

    /// <summary>
    ///     Currency format with thousand separators and 2 decimal places.
    ///     Example: $1,234.00
    ///     For use with decimal/double/float types
    /// </summary>
    public const string CURRENCY_WITH_CENTS = "C";

    /// <summary>
    ///     Currency format with 2 decimals (unit prices).
    ///     Example: $12.34
    ///     For use with decimal/double/float types
    /// </summary>
    public const string UNIT_PRICE = "C2";

    /// <summary>
    ///     Fixed-point format with 2 decimal places.
    ///     Example: 1234.00
    ///     For use with decimal/double/float types
    /// </summary>
    public const string FIXED_POINT_WITH_DECIMALS = "F2";

    /// <summary>
    ///     Custom format with thousand separators and 2 decimal places.
    ///     Example: 1,234.00
    ///     For use with decimal/double/float types
    /// </summary>
    public const string THOUSAND_SEPARATORS_WITH_DECIMALS = "#,##0.00";

    /// <summary>
    ///     Standard percentage format with 2 decimal places (multiplies by 100).
    ///     Example: For decimal value 0.1234, displays as "12.34%"
    ///     For use with decimal/double/float types (value between 0 and 1)
    /// </summary>
    public const string PERCENTAGE_WITH_DECIMALS = "P2";

    /// <summary>
    ///     Percentage format for reports (1 decimal).
    ///     Example: 12.3%
    ///     For use with decimal/double/float types (value between 0 and 1)
    /// </summary>
    public const string PERCENTAGE_REPORT = "P1";

    #endregion

    #region String/Text Only Formats (Complex patterns - requires custom implementation)

    /// <summary>
    ///     Custom format for phone numbers (US style).
    ///     Example: (123) 456-7890
    ///     NOTE: Requires special string handling/masking implementation
    ///     Not directly usable with standard numeric format - use InputBase<string> with masking
    /// </summary>
    public const string PHONE_NUMBER = "(###) ###-####";

    /// <summary>
    ///     Custom format for social security number.
    ///     Example: 123-45-6789
    ///     NOTE: Requires special string handling/masking implementation
    ///     Not directly usable with standard numeric format - use InputBase<string> with masking
    /// </summary>
    public const string SOCIAL_SECURITY_NUMBER = "###-##-####";

    /// <summary>
    ///     Custom format for credit card display.
    ///     Example: 1234 5678 9012 3456
    ///     NOTE: Requires special string handling/masking implementation
    ///     Not directly usable with standard numeric format - use InputBase<string> with masking
    /// </summary>
    public const string CREDIT_CARD = "#### #### #### ####";

    /// <summary>
    ///     Custom format for postal/zip code (9 digits with dash).
    ///     Example: 12345-6789
    ///     NOTE: Requires special string handling/masking implementation
    ///     Not directly usable with standard numeric format - use InputBase<string> with masking
    /// </summary>
    public const string ZIP_CODE9 = "00000-0000";

    #endregion
}