namespace Sigaa.Api.Common.Scraping.Configuration;

internal interface IScrapingModelConfigurationProvider
{
    ScrapingModelConfiguration GetConfiguration<TModel>() where TModel : class;
}