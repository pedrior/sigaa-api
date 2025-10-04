using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Document;

internal interface IDocument : IElement
{
    public Uri Url { get; }
    
    public ISession? Session { get; }
}