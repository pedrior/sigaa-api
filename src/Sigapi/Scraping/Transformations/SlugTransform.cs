using System.Globalization;

namespace Sigapi.Scraping.Transformations;

internal sealed class SlugTransform : IValueTransform
{
    public static SlugTransform Instance { get; } = new();
    
    private const int MaxLength = 100;
    
    public string? Transform(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        var normalized = value.Normalize(NormalizationForm.FormD);
        var slug = new StringBuilder(normalized.Length);

        var previousWasDash = false;

        foreach (var @char in normalized)
        {
            if (slug.Length >= MaxLength)
            {
                break;
            }

            // Skip all diacritical marks (e.g. accents).
            if (CharUnicodeInfo.GetUnicodeCategory(@char) is UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var lowered = char.ToLowerInvariant(@char);

            // Append letters and numbers directly.
            if (lowered is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                slug.Append(lowered);
                previousWasDash = false;
            }
            // For any other character, treat it as a separator.Only append a hyphen if the
            // previous character wasn't one, and we're not at the beginning of the string.
            else if (slug.Length > 0 && !previousWasDash)
            {
                slug.Append('-');
                previousWasDash = true;
            }
        }

        return slug.ToString();
    }
}