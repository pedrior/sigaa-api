using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping;

internal sealed class SingleEnrollmentHandler : ILoginResponseHandler
{
    private readonly IEnrollmentProvider enrollmentProvider;

    public SingleEnrollmentHandler(IEnrollmentProvider enrollmentProvider)
    {
        this.enrollmentProvider = enrollmentProvider;
    }

    public bool Evaluate(IDocument document)
    {
        var isStudentDocument = document.Url.AbsoluteUri.Contains("discente.jsf");
        var isSingleEnrollment = document.Query(EnrollmentSelector.EnrollmentSelectorLinkSelector) is null;

        return isStudentDocument && isSingleEnrollment;
    }

    public async Task<User> HandleAsync(IDocument document,
        string? enrollment = null,
        CancellationToken cancellationToken = default)
    {
        var enrollments = await enrollmentProvider.ListEnrollmentsAsync(cancellationToken);
        var foundEnrollment = enrollments.First();

        // We know that the user has only one enrollment, but we still return an error
        // if the enrollment provided by the user doesn't match the one we found.
        if (!string.IsNullOrEmpty(enrollment) && enrollment != foundEnrollment.Number)
        {
            throw new InvalidEnrollmentException();
        }

        return new User(foundEnrollment);
    }
}