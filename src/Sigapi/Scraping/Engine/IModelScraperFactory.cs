namespace Sigapi.Scraping.Engine;

internal interface IModelScraperFactory
{
    IModelScraper CreateScraper(Type modelType);
    
    IModelScraper<TModel> CreateScraper<TModel>() where TModel : class, new();
}