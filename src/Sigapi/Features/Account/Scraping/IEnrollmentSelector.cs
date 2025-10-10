using Sigapi.Features.Account.Models;
using ISession = Sigapi.Scraping.Browsing.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal interface IEnrollmentSelector
{
    Task<User> SelectAsync(ISession session,
        Enrollment enrollment,
        IEnumerable<Enrollment> enrollments,
        CancellationToken cancellationToken = default);
}