namespace Sigaa.Api.Common.Scraping.Converters;

internal interface IValueConverter
{
    object? Convert(string? value);
}