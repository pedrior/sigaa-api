using Sigapi.Scraping.Exceptions;

namespace Sigapi.Scraping.Document;

internal sealed class SelectorNotFoundException : ScrapingException
{
    public SelectorNotFoundException(string message, string? selector = null, Exception? inner = null) 
        : base(message, inner)
    {
        Selector = selector;
    }
    
    public string? Selector { get; }
}