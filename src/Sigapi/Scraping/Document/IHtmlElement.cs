namespace Sigapi.Scraping.Document;

internal interface IHtmlElement
{
    string? GetText();
    
    string? GetAttribute(string name);
    
    IHtmlElement? Query(string selector);
    
    IEnumerable<IHtmlElement> QueryAll(string selector);
}