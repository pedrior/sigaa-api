namespace Sigaa.Api.Common.Scraping;

internal interface IModelScraperFactory
{
    IModelScraper CreateScraper(Type modelType);
    
    IModelScraper<TModel> CreateScraper<TModel>() where TModel : class, new();
}