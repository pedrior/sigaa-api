using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine;

internal interface IModelScraper
{
    object Scrape(IHtmlElement element);
}

internal interface IModelScraper<out TModel> : IModelScraper where TModel : class, new()
{
    new TModel Scrape(IHtmlElement element);
}