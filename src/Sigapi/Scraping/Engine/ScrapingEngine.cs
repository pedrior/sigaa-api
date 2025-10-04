using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine;

internal sealed class ScrapingEngine : IScrapingEngine
{
    private readonly ILogger<ScrapingEngine> logger;
    private readonly IModelScraperFactory modelScraperFactory;
    private readonly IScrapingModelConfigurationProvider configProvider;

    public ScrapingEngine(ILogger<ScrapingEngine> logger,
        IModelScraperFactory modelScraperFactory,
        IScrapingModelConfigurationProvider configProvider)
    {
        this.logger = logger;
        this.modelScraperFactory = modelScraperFactory;
        this.configProvider = configProvider;
    }

    public TModel Scrape<TModel>(IElement root) where TModel : class, new()
    {
        var scraper = modelScraperFactory.CreateScraper<TModel>();
        var config = configProvider.GetConfiguration<TModel>();

        return scraper.Scrape(ResolveRootElement(root, config.Selector));
    }

    public async Task<IReadOnlyCollection<TModel>> ScrapeAllAsync<TModel>(IElement root,
        CancellationToken cancellationToken = default) where TModel : class, new()
    {
        var config = configProvider.GetConfiguration<TModel>();
        var scraper = modelScraperFactory.CreateScraper<TModel>();

        if (config.Selector is null)
        {
            throw new ScrapingConfigurationException(
                $"ScrapeAll<{typeof(TModel).Name}> must be configured with a root selector.");
        }

        var elements = root.QueryAll(config.Selector)
            .ToList();

        var results = new List<TModel>();

        logger.LogInformation(
            "Found {ElementCount} elements to scrape for model {ModelName} with selector '{Selector}'",
            elements.Count,
            typeof(TModel).Name,
            config.Selector);

        await Parallel.ForEachAsync(
            elements,
            new ParallelOptions { CancellationToken = cancellationToken },
            (element, _) =>
            {
                results.Add(scraper.Scrape(element));
                return ValueTask.CompletedTask;
            });

        return results.AsReadOnly();
    }

    private static IElement ResolveRootElement(IElement root, string? rootSelector)
    {
        var element = rootSelector is null
            ? root
            : root.Query(rootSelector);

        return element ?? throw new SelectorNotFoundException(
            $"Root element not found using selector '{rootSelector}'.",
            rootSelector);
    }
}