namespace Sigapi.Scraping.Document;

internal sealed class Document : IDocument
{
    private readonly IElement root;
    
    public Document(IElement root, Uri url)
    {
         this.root = root;
         
         Url = url;
    }
    
    public Uri Url { get; }

    public string? GetText() => root.GetText();

    public string? GetAttribute(string name) => root.GetAttribute(name);

    public IElement? Query(string selector) => root.Query(selector);

    public IElement? QueryNextSibling(string selector) => root.QueryNextSibling(selector);

    public IEnumerable<IElement> QueryAll(string selector) => root.QueryAll(selector);
    
    public IEnumerable<IElement> QueryAllNextSiblings(string selector) => root.QueryAllNextSiblings(selector);
}