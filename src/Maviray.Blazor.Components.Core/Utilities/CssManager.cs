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
    public static string Combine(params string?[]? classes)
    {
        if (classes == null || classes.Length == 0)
        {
            return string.Empty;
        }

        var classList = new HashSet<string>(StringComparer.Ordinal);

        foreach (var cssClass in classes)
        {
            if (string.IsNullOrWhiteSpace(cssClass))
            {
                continue;
            }

            // Split by whitespace and add each class individually
            var individualClasses = cssClass.Split(
                [' ', '\t', '\n', '\r'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );

            foreach (var individualClass in individualClasses)
            {
                if (!string.IsNullOrWhiteSpace(individualClass))
                {
                    classList.Add(individualClass);
                }
            }
        }

        return string.Join(" ", classList);
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