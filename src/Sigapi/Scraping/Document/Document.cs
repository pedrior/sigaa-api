using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Document;

internal sealed class Document : IDocument
{
    private readonly IHtmlElement root;
    
    public Document(IHtmlElement root, Uri url, ISession? session)
    {
         this.root = root;
         
         Url = url;
         Session = session;
    }
    
    public Uri Url { get; }
    
    public ISession? Session { get; }

    public string? GetText() => root.GetText();

    public string? GetAttribute(string name) => root.GetAttribute(name);

    public IHtmlElement? Query(string selector) => root.Query(selector);

    public IEnumerable<IHtmlElement> QueryAll(string selector) => root.QueryAll(selector);
}