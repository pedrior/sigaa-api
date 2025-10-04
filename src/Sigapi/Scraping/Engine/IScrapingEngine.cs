using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine;

internal interface IScrapingEngine
{
    TModel Scrape<TModel>(IElement root) where TModel : class, new();

    Task<IReadOnlyCollection<TModel>> ScrapeAllAsync<TModel>(IElement root,
        CancellationToken cancellationToken = default) where TModel : class, new();
}