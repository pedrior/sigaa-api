using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine;

internal interface IScrapingEngine
{
    TModel Scrape<TModel>(IHtmlElement root) where TModel : class, new();

    Task<IReadOnlyCollection<TModel>> ScrapeAllAsync<TModel>(IHtmlElement root,
        CancellationToken cancellationToken = default) where TModel : class, new();
}