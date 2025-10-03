using Sigapi.Scraping.Exceptions;

namespace Sigapi.Scraping.Configuration;

internal class ScrapingConfigurationException(string message, Exception? inner = null)
    : ScrapingException(message, inner);