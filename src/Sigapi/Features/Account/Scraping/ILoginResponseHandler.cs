using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Document;

namespace Sigapi.Features.Account.Scraping;

internal interface ILoginResponseHandler
{
    bool Evaluate(IDocument document);

    Task<User> HandleAsync(ISession session,
        IDocument document,
        string? enrollment = null,
        CancellationToken cancellationToken = default);
}