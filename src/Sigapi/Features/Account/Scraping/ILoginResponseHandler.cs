using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Document;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal interface ILoginResponseHandler
{
    bool Evaluate(IDocument page);

    Task<User> HandleAsync(ISession session,
        IDocument page,
        string? enrollment = null,
        CancellationToken cancellationToken = default);
}