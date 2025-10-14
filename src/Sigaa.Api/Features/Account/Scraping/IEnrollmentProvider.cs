using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping;

internal interface IEnrollmentProvider
{
    Task<IEnumerable<Enrollment>> ListEnrollmentsAsync(ISession session,
        CancellationToken cancellationToken = default);
}