using Sigapi.Features.Account.Exceptions;
using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Document;
using ISession = Sigapi.Scraping.Browsing.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal sealed class MultipleEnrollmentHandler : ILoginResponseHandler
{
    private readonly IEnrollmentProvider enrollmentProvider;
    private readonly IEnrollmentSelector enrollmentSelector;

    public MultipleEnrollmentHandler(IEnrollmentProvider enrollmentProvider,
        IEnrollmentSelector enrollmentSelector)
    {
        this.enrollmentProvider = enrollmentProvider;
        this.enrollmentSelector = enrollmentSelector;
    }

    public bool Evaluate(IDocument document)
    {
        var isEnrollmentSelectorDocument = document.Url.AbsoluteUri.Contains("vinculos.jsf");
        var isMultipleEnrollment = document.Query(EnrollmentSelector.EnrollmentSelectorLinkSelector) is not null;

        return isEnrollmentSelectorDocument || isMultipleEnrollment;
    }

    public async Task<User> HandleAsync(ISession session,
        IDocument document,
        string? enrollment = null,
        CancellationToken cancellationToken = default)
    {
        var enrollments = await enrollmentProvider.ListEnrollmentsAsync(session, cancellationToken);
        var enrollmentsArray = enrollments as Enrollment[] ?? enrollments.ToArray();

        var enrollmentToSelect = FindEnrollmentToSelect(enrollmentsArray, enrollment);
        var user = await enrollmentSelector.SelectAsync(
            session,
            enrollmentToSelect,
            enrollmentsArray,
            cancellationToken);

        return user;
    }

    private static Enrollment FindEnrollmentToSelect(Enrollment[] enrollments, string? targetEnrollment)
    {
        if (string.IsNullOrEmpty(targetEnrollment))
        {
            return enrollments.OrderByDescending(e => e.Number).First();
        }

        var enrollment = enrollments.SingleOrDefault(e => e.Number == targetEnrollment);
        return enrollment ?? throw new InvalidEnrollmentException();
    }
}