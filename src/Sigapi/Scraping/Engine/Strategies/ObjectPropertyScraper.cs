using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine.Strategies;

internal sealed class ObjectPropertyScraper : PropertyScraper<ObjectPropertyScrapingConfiguration>
{
    private readonly IModelScraperFactory modelScraperFactory;

    public ObjectPropertyScraper(IConversionService conversionService, 
        IModelScraperFactory modelScraperFactory) : base(conversionService)
    {
        this.modelScraperFactory = modelScraperFactory;
    }

    protected override void ProcessProperty(object model,
        ObjectPropertyScrapingConfiguration config,
        IElement parent)
    {
        var element = ResolveElement(parent, config);
        if (element is null)
        {
            if (!config.IsOptional)
            {
                throw new SelectorNotFoundException(
                    $"Element not found for object property '{config.Property.Name}'.",
                    config.Selector);
            }

            return;
        }

        var scraper = modelScraperFactory.CreateScraper(config.Property.PropertyType);
        var value = scraper.Scrape(element);

        config.Property.SetValue(model, value);
    }
}