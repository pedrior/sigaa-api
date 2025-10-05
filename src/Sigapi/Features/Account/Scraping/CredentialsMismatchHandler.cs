using Sigapi.Features.Account.Exceptions;
using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Document;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal sealed class CredentialsMismatchHandler : ILoginResponseHandler
{
    public bool Evaluate(IDocument page) => page.Url.AbsoluteUri.Contains("logon.jsf");

    public Task<User> HandleAsync(ISession session,
        IDocument page,
        string? enrollment = null,
        CancellationToken cancellationToken = default) => throw new InvalidCredentialsException();
}