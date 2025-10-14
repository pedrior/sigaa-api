using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Converters;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping.Strategies;

internal sealed class ValuePropertyScraper : PropertyScraper<ValuePropertyScrapingConfiguration>
{
    public ValuePropertyScraper(IConversionService conversionService) : base(conversionService)
    {
    }

    protected override void ProcessProperty(object model,
        ValuePropertyScrapingConfiguration config,
        IElement parent)
    {
        var element = ResolveElement(parent, config);
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