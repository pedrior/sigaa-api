namespace Sigaa.Api.Common.Scraping.Configuration;

internal sealed record ScrapingModelConfiguration(string? Selector, 
    IReadOnlyCollection<PropertyScrapingConfiguration> Properties);