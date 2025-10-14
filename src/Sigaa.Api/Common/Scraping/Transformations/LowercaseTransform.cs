namespace Sigaa.Api.Common.Scraping.Transformations;

internal sealed class LowercaseTransform : IValueTransform
{
    public static LowercaseTransform Instance { get; } = new();
    
    public string? Transform(string? value) => value?.ToLowerInvariant();
}