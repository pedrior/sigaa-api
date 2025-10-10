using Sigapi.Features.Account.Models;

namespace Sigapi.Features.Account.Scraping;

internal interface IEnrollmentProvider
{
    Task<IEnumerable<Enrollment>> ListEnrollmentsAsync(ISession session,
        CancellationToken cancellationToken = default);
}