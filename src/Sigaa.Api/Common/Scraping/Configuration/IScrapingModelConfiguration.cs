using Sigaa.Api.Common.Scraping.Builders;

namespace Sigaa.Api.Common.Scraping.Configuration;

internal interface IScrapingModelConfiguration<TModel> where TModel : class
{
    void Configure(ScrapingModelBuilder<TModel> builder);
}