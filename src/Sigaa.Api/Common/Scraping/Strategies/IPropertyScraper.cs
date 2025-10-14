using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping.Strategies;

internal interface IPropertyScraper
{
    bool Evaluate(PropertyScrapingConfiguration config);
    
    void Execute(object model, PropertyScrapingConfiguration config, IElement parent);
}