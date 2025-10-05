namespace Sigapi.Scraping.Document;

internal sealed class Document : IDocument
{
    private static readonly Uri Blank = new("about:blank"); 
    
    private readonly IElement root;
    
    public Document(IElement root)
    {
         this.root = root;
    }

    public Uri Url { get; set; } = Blank;

    public string? GetText() => root.GetText();

    public string? GetAttribute(string name) => root.GetAttribute(name);

    public IElement? Query(string selector) => root.Query(selector);

    public IElement? QueryNextSibling(string selector) => root.QueryNextSibling(selector);

    public IEnumerable<IElement> QueryAll(string selector) => root.QueryAll(selector);
    
    public IEnumerable<IElement> QueryAllNextSiblings(string selector) => root.QueryAllNextSiblings(selector);
}