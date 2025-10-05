using Sigapi.Features.Account.Exceptions;
using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Document;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal sealed class SingleEnrollmentHandler : ILoginResponseHandler
{
    private readonly IEnrollmentProvider enrollmentProvider;

    public SingleEnrollmentHandler(IEnrollmentProvider enrollmentProvider)
    {
        this.enrollmentProvider = enrollmentProvider;
    }

    public bool Evaluate(IDocument page)
    {
        var isStudentPage = page.Url.AbsoluteUri.Contains("discente.jsf");
        var isSingleEnrollment = page.Query(EnrollmentSelector.EnrollmentSelectorLinkSelector) is null;

        return isStudentPage && isSingleEnrollment;
    }

    public async Task<User> HandleAsync(ISession session,
        IDocument page,
        string? enrollment = null,
        CancellationToken cancellationToken = default)
    {
        var enrollments = await enrollmentProvider.ListEnrollmentsAsync(session, cancellationToken);
        var foundEnrollment = enrollments.First();

        // We know that the user has only one enrollment, but we still return an error
        // if the enrollment provided by the user doesn't match the one we found.
        if (enrollment is not null && enrollment != foundEnrollment.Number)
        {
            throw new InvalidEnrollmentException();
        }

        return new User(foundEnrollment);
    }
}