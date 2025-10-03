using Sigapi.Features.Account.Models;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal interface IEnrollmentProvider
{
    Task<IEnumerable<Enrollment>> ListEnrollmentsAsync(ISession session,
        CancellationToken cancellationToken = default);
}