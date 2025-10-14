using Sigaa.Api.Common.Scraping.Configuration;

namespace Sigaa.Api.Common.Scraping.Builders;

internal interface IPropertyBuilder
{
    PropertyScrapingConfiguration BuildConfiguration();
}