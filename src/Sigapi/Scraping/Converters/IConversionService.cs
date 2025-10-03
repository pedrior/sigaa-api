namespace Sigapi.Scraping.Converters;

internal interface IConversionService
{
    object? Convert(Type targetType, string? value, IValueConverter? customConverter);
}