using Sigapi.Scraping.Configuration;

namespace Sigapi.Scraping.Builders;

internal interface IPropertyBuilder
{
    PropertyScrapingConfiguration BuildConfiguration();
}