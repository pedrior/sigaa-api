using System.Runtime.CompilerServices;
using Sigapi.Scraping.Document;
using ISession = Sigapi.Scraping.Browsing.Sessions.ISession;

namespace Sigapi.Scraping.Browsing;

internal interface IDocumentRequest
{
    IDocumentRequest WithSession(ISession session, CancellationToken cancellationToken = default);
    
    IDocumentRequest WithUserSession(CancellationToken cancellationToken = default);
    
    IDocumentRequest WithContextualSession(CancellationToken cancellationToken = default);

    IDocumentRequest WithAnonymousSession(CancellationToken cancellationToken = default);
    
    IDocumentRequest WithFormData(IReadOnlyDictionary<string, string> data);
    
    TaskAwaiter<IDocument> GetAwaiter();

    Task<IDocument> AsTask();
}