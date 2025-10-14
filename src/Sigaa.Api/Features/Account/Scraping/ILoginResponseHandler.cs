using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping;

internal interface ILoginResponseHandler
{
    bool Evaluate(IDocument document);

    Task<User> HandleAsync(ISession session,
        IDocument document,
        string? enrollment = null,
        CancellationToken cancellationToken = default);
}