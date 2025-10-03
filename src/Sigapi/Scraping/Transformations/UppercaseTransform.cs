namespace Sigapi.Scraping.Transformations;

internal sealed class UppercaseTransform : IValueTransform
{
    public static UppercaseTransform Instance { get; } = new();
    
    public string? Transform(string? value) => value?.ToUpperInvariant();
}