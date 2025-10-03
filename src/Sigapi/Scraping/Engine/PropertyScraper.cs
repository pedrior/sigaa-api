using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine;

internal abstract class PropertyScraper<TConfig> : IPropertyScraper where TConfig : PropertyScrapingConfiguration
{
    private readonly IConversionService conversionService;

    protected PropertyScraper(IConversionService conversionService)
    {
        this.conversionService = conversionService;
    }

    public bool Evaluate(PropertyScrapingConfiguration config) => config is TConfig;

    public void Execute(object model, PropertyScrapingConfiguration config, IHtmlElement parent)
    {
        if (config is TConfig typedConfig)
        {
            ProcessProperty(model, typedConfig, parent);
        }
    }

    protected abstract void ProcessProperty(object model, TConfig config, IHtmlElement parent);

    protected static IHtmlElement? ResolveElement(IHtmlElement element, string? selector)
    {
        return string.IsNullOrWhiteSpace(selector)
            ? element
            : element.Query(selector);
    }

    protected static string? ExtractRawValue(IHtmlElement? element, string? attribute)
    {
        if (element is null)
        {
            return null;
        }

        return attribute is null
            ? element.GetText()
            : element.GetAttribute(attribute);
    }

    protected object? ConvertValue(Type targetType,
        string? rawValue,
        object? defaultValue,
        IValueConverter? valueConverter)
    {
        try
        {
            return conversionService.Convert(targetType, rawValue, valueConverter);
        }
        catch (Exception)
        {
            if (defaultValue is null)
            {
                throw; // Re-throw if no default value is available.
            }

            return defaultValue;
        }
    }
}