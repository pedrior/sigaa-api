namespace Sigaa.Api.Common.Scraping.Transformations;

internal interface IValueTransform
{
    string? Transform(string? value);
}