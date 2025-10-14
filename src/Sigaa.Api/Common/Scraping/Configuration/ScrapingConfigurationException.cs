using Sigaa.Api.Common.Scraping.Exceptions;

namespace Sigaa.Api.Common.Scraping.Configuration;

internal class ScrapingConfigurationException(string message, Exception? inner = null)
    : ScrapingException(message, inner);