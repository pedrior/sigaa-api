namespace Sigapi.Scraping.Configuration;

internal sealed class InvalidConfigurationException(string message, Exception? inner = null)
    : ScrapingConfigurationException(message, inner);