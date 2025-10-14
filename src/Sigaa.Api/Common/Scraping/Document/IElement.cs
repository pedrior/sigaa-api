namespace Sigaa.Api.Common.Scraping.Document;

internal interface IElement
{
    string? GetText();
    
    string? GetAttribute(string name);
    
    IElement? Query(string selector);
    
    IElement? QueryNextSibling(string selector);
    
    IEnumerable<IElement> QueryAll(string selector);
    
    IEnumerable<IElement> QueryAllNextSiblings(string selector);
}