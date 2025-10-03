using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Document;

namespace Sigapi.Features.Account.Scraping;

internal interface ILoginResponseHandler
{
    bool Evaluate(IDocument page);

    Task<User> HandleAsync(IDocument page,
        string? enrollment = null,
        CancellationToken cancellationToken = default);
}