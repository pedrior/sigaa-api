using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine;

internal sealed class ValuePropertyScraper : PropertyScraper<ValuePropertyScrapingConfiguration>
{
    public ValuePropertyScraper(IConversionService conversionService) : base(conversionService)
    {
    }

    protected override void ProcessProperty(object model,
        ValuePropertyScrapingConfiguration config,
        IHtmlElement parent)
    {
        var element = ResolveElement(parent, config.Selector);
        if (element is null)
        {
            if (config.DefaultValue is not null)
            {
                config.Property.SetValue(model, config.DefaultValue);
                return;
            }

            if (!config.IsOptional)
            {
                throw new SelectorNotFoundException(
                    $"Element not found for property '{config.Property.Name}'.",
                    config.Selector);
            }

            return;
        }

        var rawValue = ExtractRawValue(element, config.Attribute);
        var transformedValue = config.Transformations.Aggregate(
            rawValue,
            (current, transform) => transform.Transform(current));

        var convertedValue = ConvertValue(
            config.Property.PropertyType,
            transformedValue,
            config.DefaultValue,
            config.Converter);

        config.Property.SetValue(model, convertedValue);
    }
}