namespace Sigapi.Scraping.Configuration;

internal sealed record ScrapingModelConfiguration(string? Selector, 
    IReadOnlyCollection<PropertyScrapingConfiguration> Properties);