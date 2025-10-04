using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine;

internal interface IModelScraper
{
    object Scrape(IElement element);
}

internal interface IModelScraper<out TModel> : IModelScraper where TModel : class, new()
{
    new TModel Scrape(IElement element);
}