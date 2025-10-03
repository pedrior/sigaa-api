namespace Sigapi.Scraping.Converters;

internal interface IValueConverter
{
    object? Convert(string? value);
}