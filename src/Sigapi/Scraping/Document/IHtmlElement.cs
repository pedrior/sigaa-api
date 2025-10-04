namespace Sigapi.Scraping.Document;

internal interface IHtmlElement
{
    string? GetText();
    
    string? GetAttribute(string name);
    
    IHtmlElement? Query(string selector);
    
    IHtmlElement? QueryNextSibling(string selector);
    
    IEnumerable<IHtmlElement> QueryAll(string selector);
    
    IEnumerable<IHtmlElement> QueryAllNextSiblings(string selector);
}