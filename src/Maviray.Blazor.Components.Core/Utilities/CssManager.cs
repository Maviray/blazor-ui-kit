using System.Buffers;

namespace Maviray.Blazor.Components.Core.Utilities;

/// <summary>
///     Utility class for managing and combining CSS class strings.
/// </summary>
public static class CssManager
{
    /// <summary>
    ///     Combines multiple CSS class strings into a single space-separated string.
    ///     Handles null, empty, and whitespace values, and removes duplicates.
    /// </summary>
    /// <param name="classes">Variable number of CSS class strings to combine.</param>
    /// <returns>A single combined CSS class string with duplicates removed.</returns>
    public static string Combine(params string?[]? classes) =>
        classes is null ? string.Empty : Combine(classes.AsSpan());

    /// <summary>
    ///     Combines multiple CSS class strings into a single space-separated string, keeping the first
    ///     occurrence of each class. Allocates only the result: classes are split and compared as spans.
    /// </summary>
    public static string Combine(params ReadOnlySpan<string?> classes)
    {
        var capacity = 0;

        foreach (var cssClass in classes)
        {
            capacity += cssClass is null ? 0 : cssClass.Length + 1;
        }

        if (capacity == 0)
        {
            return string.Empty;
        }

        char[]? rented = null;
        var buffer = capacity <= STACK_BUFFER_LIMIT
            ? stackalloc char[capacity]
            : rented = ArrayPool<char>.Shared.Rent(capacity);

        try
        {
            var length = 0;

            foreach (var cssClass in classes)
            {
                var remaining = cssClass.AsSpan();

                while (true)
                {
                    var start = remaining.IndexOfAnyExcept(ClassSeparators);

                    if (start < 0)
                    {
                        break;
                    }

                    remaining = remaining[start..];
                    var end = remaining.IndexOfAny(ClassSeparators);
                    var token = end < 0 ? remaining : remaining[..end];
                    remaining = end < 0 ? ReadOnlySpan<char>.Empty : remaining[end..];

                    if (ContainsClass(buffer[..length], token))
                    {
                        continue;
                    }

                    if (length > 0)
                    {
                        buffer[length++] = ' ';
                    }

                    token.CopyTo(buffer[length..]);
                    length += token.Length;
                }
            }

            return length == 0 ? string.Empty : new(buffer[..length]);
        }
        finally
        {
            if (rented is not null)
            {
                ArrayPool<char>.Shared.Return(rented);
            }
        }
    }

    private const int STACK_BUFFER_LIMIT = 512;

    private static readonly SearchValues<char> ClassSeparators = SearchValues.Create(" \t\n\r");

    // Class lists are short, so a linear scan of what was written beats a hash set's allocations.
    private static bool ContainsClass(ReadOnlySpan<char> written, ReadOnlySpan<char> cssClass)
    {
        while (!written.IsEmpty)
        {
            var end = written.IndexOf(' ');
            var current = end < 0 ? written : written[..end];

            if (current.SequenceEqual(cssClass))
            {
                return true;
            }

            if (end < 0)
            {
                break;
            }

            written = written[(end + 1)..];
        }

        return false;
    }

    /// <summary>
    ///     Combines multiple CSS class strings without removing duplicates.
    ///     Useful when order matters or duplicate classes are intentional.
    /// </summary>
    /// <param name="classes">Variable number of CSS class strings to combine.</param>
    /// <returns>A single combined CSS class string preserving duplicates.</returns>
    public static string CombinePreserveDuplicates(params string?[]? classes)
    {
        if (classes == null || classes.Length == 0)
        {
            return string.Empty;
        }

        var result = string.Join(" ",
            classes.Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c?.Trim())
        );

        return result;
    }

    /// <summary>
    ///     Conditionally combines CSS classes based on boolean conditions.
    /// </summary>
    /// <param name="conditionalClasses">Tuples of (condition, cssClass) pairs.</param>
    /// <returns>A combined CSS class string containing only classes where condition is true.</returns>
    public static string CombineConditional(params (bool condition, string cssClass)[]? conditionalClasses)
    {
        if (conditionalClasses == null || conditionalClasses.Length == 0)
        {
            return string.Empty;
        }

        var classes = conditionalClasses
            .Where(tuple => tuple.condition && !string.IsNullOrWhiteSpace(tuple.cssClass))
            .Select(tuple => tuple.cssClass);

        return Combine(classes.ToArray());
    }

    /// <summary>
    ///     Merges CSS classes with a base class, allowing overrides.
    /// </summary>
    /// <param name="baseClass">The base CSS class string.</param>
    /// <param name="additionalClasses">Additional CSS classes to merge.</param>
    /// <returns>A combined CSS class string.</returns>
    public static string Merge(string? baseClass, params string?[] additionalClasses)
    {
        var allClasses = new List<string?> { baseClass };
        allClasses.AddRange(additionalClasses);
        return Combine(allClasses.ToArray());
    }

    /// <summary>
    ///     Toggles a CSS class based on a condition.
    /// </summary>
    /// <param name="baseClass">The base CSS class string.</param>
    /// <param name="toggleClass">The class to toggle.</param>
    /// <param name="condition">Whether to include the toggle class.</param>
    /// <returns>A combined CSS class string.</returns>
    public static string Toggle(string? baseClass, string toggleClass, bool condition) =>
        condition
            ? Combine(baseClass, toggleClass)
            : baseClass ?? string.Empty;

    /// <summary>
    ///     Removes specific CSS classes from a class string.
    /// </summary>
    /// <param name="cssClass">The original CSS class string.</param>
    /// <param name="classesToRemove">Classes to remove.</param>
    /// <returns>A CSS class string with specified classes removed.</returns>
    public static string Remove(string? cssClass, params string[]? classesToRemove)
    {
        if (string.IsNullOrWhiteSpace(cssClass))
        {
            return string.Empty;
        }

        if (classesToRemove == null || classesToRemove.Length == 0)
        {
            return cssClass;
        }

        var classes = cssClass.Split(
            [' ', '\t', '\n', '\r'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

        var removeSet = new HashSet<string>(classesToRemove, StringComparer.Ordinal);
        var result = classes.Where(c => !removeSet.Contains(c));

        return string.Join(" ", result);
    }

    /// <summary>
    ///     Checks if a CSS class string contains a specific class.
    /// </summary>
    /// <param name="cssClass">The CSS class string to check.</param>
    /// <param name="classToFind">The class to find.</param>
    /// <returns>True if the class is found, otherwise false.</returns>
    public static bool Contains(string? cssClass, string classToFind)
    {
        if (string.IsNullOrWhiteSpace(cssClass) || string.IsNullOrWhiteSpace(classToFind))
        {
            return false;
        }

        var classes = cssClass.Split(
            [' ', '\t', '\n', '\r'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

        return classes.Contains(classToFind, StringComparer.Ordinal);
    }
}