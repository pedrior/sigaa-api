namespace Sigapi.Scraping.Exceptions;

internal class ScrapingException(string message, Exception? inner = null) : Exception(message, inner);