namespace Sigaa.Api.Common.Scraping.Transformations;

internal sealed class RegexCaptureTransform : IValueTransform
{
    private readonly Regex regex;
    private readonly int group;
    private readonly string? groupName;

    public RegexCaptureTransform(Regex regex, int group = 1)
    {
        this.regex = regex;
        this.group = group;
    }
    
    public RegexCaptureTransform(Regex regex, string groupName)
    {
        this.regex = regex;
        this.groupName = groupName;
    }

    public string? Transform(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var match = regex.Match(value);
        if (!match.Success)
        {
            return null;
        }
        
        // Choose between group name or index.
        return groupName is not null
            ? match.Groups[groupName].Value
            : match.Groups[group].Value;
    }
}