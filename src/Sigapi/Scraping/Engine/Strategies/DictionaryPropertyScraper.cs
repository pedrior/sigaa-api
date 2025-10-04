using System.Collections;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Exceptions;

namespace Sigapi.Scraping.Engine.Strategies;

internal sealed class DictionaryPropertyScraper : PropertyScraper<DictionaryPropertyScrapingConfiguration>
{
    public DictionaryPropertyScraper(IConversionService conversionService) : base(conversionService)
    {
    }

    protected override void ProcessProperty(object model,
        DictionaryPropertyScrapingConfiguration config,
        IElement parent)
    {
        var elements = ResolveElements(parent, config).ToArray();
        if (elements is [])
        {
            return;
        }

        var dictionary = CreateDictionaryInstance(config.Property.PropertyType);

        foreach (var element in elements)
        {
            var keyRaw = ExtractRawValue(element, config.KeyAttribute!);
            var valueRaw = ExtractRawValue(element, config.ValueAttribute!);

            if (string.IsNullOrWhiteSpace(keyRaw))
            {
                throw new ScrapingConfigurationException(
                    $"Key attribute '{config.KeyAttribute}' is required for dictionary property '{config.Property.Name}'.");
            }

            try
            {
                var keyTransformed = config.KeyTransformations.Aggregate(
                    keyRaw,
                    (current, transform) => transform.Transform(current) ?? string.Empty);

                var valueTransformed = config.ValueTransformations.Aggregate(
                    valueRaw,
                    (current, transform) => transform.Transform(current));

                var key = ConvertValue(config.KeyType, keyTransformed, null, config.KeyConverter);
                var value = ConvertValue(config.ValueType, valueTransformed, null, config.ValueConverter);

                if (value is null && config.DefaultValue is not null)
                {
                    value = config.DefaultValue;
                }

                if (key is not null)
                {
                    dictionary[key] = value;
                }
            }
            catch (ScrapingConversionException ex)
            {
                throw new ScrapingException(
                    $"Failed to convert key/value for dictionary property '{config.Property.Name}'. " +
                    $"Key: '{keyRaw}', Value: '{valueRaw}'.",
                    ex);
            }
        }

        config.Property.SetValue(model, dictionary);
    }

    private static IDictionary CreateDictionaryInstance(Type dictionaryType)
    {
        var (keyType, valueType) = GetDictionaryTypes(dictionaryType);
        var genericType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);

        return (IDictionary)Activator.CreateInstance(genericType)!;
    }

    private static (Type keyType, Type valueType) GetDictionaryTypes(Type dictionaryType)
    {
        var @interface = dictionaryType.GetInterface(typeof(IDictionary<,>).Name)!;
        var arguments = @interface.GetGenericArguments();

        return (arguments[0], arguments[1]);
    }
}