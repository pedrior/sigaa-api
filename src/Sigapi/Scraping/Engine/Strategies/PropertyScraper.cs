using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine.Strategies;

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

    protected static IHtmlElement? ResolveElement(IHtmlElement element, PropertyScrapingConfiguration config)
    {
        var selector = config.Selector;
        if (string.IsNullOrWhiteSpace(selector))
        {
            // When the selector is empty, it refers to the element itself, regardless of the strategy.
            return element;
        }

        return config.SelectorStrategy switch
        {
            SelectorStrategy.Nested => element.Query(selector),
            SelectorStrategy.Sibling => element.QueryNextSibling(selector),
            _ => throw new NotSupportedException($"Selector strategy '{config.SelectorStrategy}' is not supported.")
        };
    }
    
    protected static IEnumerable<IHtmlElement> ResolveElements(IHtmlElement element, PropertyScrapingConfiguration config)
    {
        var selector = config.Selector;
        if (string.IsNullOrWhiteSpace(selector))
        {
            return [element];
        }

        return config.SelectorStrategy switch
        {
            SelectorStrategy.Nested => element.QueryAll(selector),
            SelectorStrategy.Sibling => element.QueryAllNextSiblings(selector),
            _ => throw new NotSupportedException($"Selector strategy '{config.SelectorStrategy}' is not supported.")
        };
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