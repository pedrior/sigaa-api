namespace Sigapi.Scraping.Transformations;

internal interface IValueTransform
{
    string? Transform(string? value);
}