using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping;

internal sealed class CredentialsMismatchHandler : ILoginResponseHandler
{
    public bool Evaluate(IDocument document) => document.Url.AbsoluteUri.Contains("logon.jsf");

    public Task<User> HandleAsync(ISession session,
        IDocument document,
        string? enrollment = null,
        CancellationToken cancellationToken = default) => throw new InvalidCredentialsException();
}