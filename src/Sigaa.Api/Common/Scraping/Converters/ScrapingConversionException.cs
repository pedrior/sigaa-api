using Sigaa.Api.Common.Scraping.Exceptions;

namespace Sigaa.Api.Common.Scraping.Converters;

internal class ScrapingConversionException(string message, Exception? inner = null) 
    : ScrapingException(message, inner);