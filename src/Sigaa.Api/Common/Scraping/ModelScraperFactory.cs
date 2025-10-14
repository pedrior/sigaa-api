namespace Sigaa.Api.Common.Scraping;

internal sealed class ModelScraperFactory(IServiceProvider serviceProvider) : IModelScraperFactory
{
    public IModelScraper CreateScraper(Type modelType)
    {
        var scraperType = typeof(IModelScraper<>).MakeGenericType(modelType);
        return (IModelScraper)serviceProvider.GetRequiredService(scraperType);
    }

    public IModelScraper<TModel> CreateScraper<TModel>() where TModel : class, new() =>
        (IModelScraper<TModel>)CreateScraper(typeof(TModel));
}