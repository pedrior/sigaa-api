using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Document;

internal interface IDocument : IHtmlElement
{
    public Uri Url { get; }
    
    public ISession? Session { get; }
}