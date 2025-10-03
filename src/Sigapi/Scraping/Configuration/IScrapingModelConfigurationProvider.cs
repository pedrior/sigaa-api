namespace Sigapi.Scraping.Configuration;

internal interface IScrapingModelConfigurationProvider
{
    ScrapingModelConfiguration GetConfiguration<TModel>() where TModel : class;
}