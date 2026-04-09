using System.Text.RegularExpressions;

namespace Maviray.Blazor.Components.Core.Extensions;

public static class StringExtensions
{
    public const int MAX_CHAR_LIMIT = 300;
    public const int DEFAULT_CHAR_LIMIT = 30;

    private static readonly Regex HexColorRegex = new Regex(
        @"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{4}|[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$",
        RegexOptions.Compiled
    );

    extension(string? text)
    {
        public string LimitToMaxCharacters(int limit = DEFAULT_CHAR_LIMIT)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (limit is <= 0 or > MAX_CHAR_LIMIT)
            {
                limit = DEFAULT_CHAR_LIMIT;
            }

            return text.Length > limit ? text[..limit] + "..." : text;
        }

        public string TrimToLengthWithDots(int length)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text.Length <= length ? text : $"{text[..length]}...";
        }

        public bool IsValidHexColor()
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return HexColorRegex.IsMatch(text);
        }
    }
}