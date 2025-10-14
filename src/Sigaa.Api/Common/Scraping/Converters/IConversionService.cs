namespace Sigaa.Api.Common.Scraping.Converters;

internal interface IConversionService
{
    object? Convert(Type targetType, string? value, IValueConverter? customConverter);
}