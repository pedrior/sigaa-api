using System.ComponentModel;
using System.Globalization;
using Sigaa.Api.Common.Scraping.Exceptions;

namespace Sigaa.Api.Common.Scraping.Converters;

internal sealed class ConversionService : IConversionService
{
    public object? Convert(Type targetType, string? value, IValueConverter? customConverter)
    {
        try
        {
            // Use the custom converter if provided.
            if (customConverter is not null)
            {
                return customConverter.Convert(value);
            }

            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (value is null)
            {
                if (nonNullableType != targetType)
                {
                    return null; // Nullable type.
                }

                if (!targetType.IsValueType)
                {
                    return null; // Reference type.
                }
            }

            // Handle direct assignment if types are already compatible.
            if (targetType.IsInstanceOfType(value))
            {
                return value == string.Empty 
                    ? null
                    : value;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                // If the target type can be null, return null.
                if (nonNullableType != targetType || !targetType.IsValueType)
                {
                    return null;
                }

                throw new ScrapingConversionException(
                    $"Cannot convert null/empty string to non-nullable type {nonNullableType.Name}.");
            }

            if (nonNullableType.IsEnum)
            {
                if (Enum.TryParse(nonNullableType, value, true, out var enumValue))
                {
                    return enumValue;
                }

                throw new ScrapingConversionException(
                    $"Cannot convert string '{value}' to enum {nonNullableType.Name}.");
            }

            // Fallback to the framework's TypeConverter.
            var converter = TypeDescriptor.GetConverter(nonNullableType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
            }

            throw new ScrapingConversionException($"No type converter found to convert string to {targetType.Name}.");
        }
        catch (Exception ex) when (ex is not ScrapingException)
        {
            throw new ScrapingConversionException($"Conversion to {targetType.Name} failed for value '{value}'.", ex);
        }
    }
}