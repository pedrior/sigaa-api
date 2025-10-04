using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine.Strategies;

namespace Sigapi.Scraping.Engine;

internal sealed class ModelScraper<TModel> : IModelScraper<TModel> where TModel : class, new()
{
    private readonly IEnumerable<IPropertyScraper> scrapers;
    private readonly IScrapingModelConfigurationProvider configProvider;

    public ModelScraper(IEnumerable<IPropertyScraper> scrapers, IScrapingModelConfigurationProvider configProvider)
    {
        this.scrapers = scrapers;
        this.configProvider = configProvider;
    }

    public TModel Scrape(IHtmlElement element)
    {
        var model = new TModel();
        var config = configProvider.GetConfiguration<TModel>();

        foreach (var propertyConfig in config.Properties)
        {
            var scraper = scrapers.FirstOrDefault(s => s.Evaluate(propertyConfig));
            scraper?.Execute(model, propertyConfig, element);
        }

        return model;
    }

    object IModelScraper.Scrape(IHtmlElement element) => Scrape(element);
}