using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Engine.Strategies;

internal interface IPropertyScraper
{
    bool Evaluate(PropertyScrapingConfiguration config);
    
    void Execute(object model, PropertyScrapingConfiguration config, IElement parent);
}