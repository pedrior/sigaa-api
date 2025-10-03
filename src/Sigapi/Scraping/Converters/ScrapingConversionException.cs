using Sigapi.Scraping.Exceptions;

namespace Sigapi.Scraping.Converters;

internal class ScrapingConversionException(string message, Exception? inner = null) 
    : ScrapingException(message, inner);