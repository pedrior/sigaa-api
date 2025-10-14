using System.Runtime.CompilerServices;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping.Browsing;

using ISession = Sessions.ISession;

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