using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping;

internal interface IScrapingEngine
{
    TModel Scrape<TModel>(IElement root) where TModel : class, new();

    Task<IReadOnlyCollection<TModel>> ScrapeAllAsync<TModel>(IElement root,
        CancellationToken cancellationToken = default) where TModel : class, new();
}