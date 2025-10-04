using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Document;

internal sealed class Document : IDocument
{
    private readonly IElement root;
    
    public Document(IElement root, Uri url, ISession? session)
    {
         this.root = root;
         
         Url = url;
         Session = session;
    }
    
    public Uri Url { get; }
    
    public ISession? Session { get; }

    public string? GetText() => root.GetText();

    public string? GetAttribute(string name) => root.GetAttribute(name);

    public IElement? Query(string selector) => root.Query(selector);

    public IElement? QueryNextSibling(string selector) => root.QueryNextSibling(selector);

    public IEnumerable<IElement> QueryAll(string selector) => root.QueryAll(selector);
    
    public IEnumerable<IElement> QueryAllNextSiblings(string selector) => root.QueryAllNextSiblings(selector);
}