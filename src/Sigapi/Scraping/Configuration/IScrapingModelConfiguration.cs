using Sigapi.Scraping.Builders;

namespace Sigapi.Scraping.Configuration;

internal interface IScrapingModelConfiguration<TModel> where TModel : class
{
    void Configure(ScrapingModelBuilder<TModel> builder);
}