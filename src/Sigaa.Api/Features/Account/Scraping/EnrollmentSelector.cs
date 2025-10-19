using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Common.Scraping.Exceptions;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping;

internal sealed class EnrollmentSelector : IEnrollmentSelector
{
    public const string EnrollmentSelectorLinkSelector = "a[href*='vinculos/listar']";

    private readonly IFetcher fetcher;

    public EnrollmentSelector(IFetcher fetcher)
    {
        this.fetcher = fetcher;
    }

    public async Task<User> SelectAsync(Enrollment enrollment,
        IEnumerable<Enrollment> enrollments,
        CancellationToken cancellationToken = default)
    {
        var document = await fetcher.FetchDocumentAsync(AccountPages.EnrollmentSelector, cancellationToken)
            .WithFormData(enrollment.Data)
            .WithPersistentSession();

        return document.Url.AbsoluteUri.Contains("discente.jsf")
            ? new User(enrollment, enrollments)
            : throw new ScrapingException(
                $"Unexpected response after submitting enrollment selector form: {document.Url}.");
    }
}