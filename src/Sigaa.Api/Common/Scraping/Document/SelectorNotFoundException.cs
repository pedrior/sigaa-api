using Sigaa.Api.Common.Scraping.Exceptions;

namespace Sigaa.Api.Common.Scraping.Document;

internal sealed class SelectorNotFoundException : ScrapingException
{
    public SelectorNotFoundException(string message, string? selector = null, Exception? inner = null) 
        : base(message, inner)
    {
        Selector = selector;
    }
    
    public string? Selector { get; }
}