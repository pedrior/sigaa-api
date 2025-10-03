using System.Text.RegularExpressions;

namespace Sigapi.Scraping.Transformations;

internal sealed class RegexReplaceTransform : IValueTransform
{
    private readonly Regex regex;
    private readonly string replacement;

    public RegexReplaceTransform(Regex regex, string? replacement = null)
    {
        this.regex = regex;
        this.replacement = replacement ?? string.Empty;
    }

    public string? Transform(string? value)
    {
        return value is null
            ? null
            : regex.Replace(value, replacement);
    }
}