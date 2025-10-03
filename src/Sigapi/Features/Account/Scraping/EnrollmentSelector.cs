using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Exceptions;
using Sigapi.Scraping.Networking;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal sealed class EnrollmentSelector : IEnrollmentSelector
{
    public const string EnrollmentSelectorLinkSelector = "a[href*='vinculos/listar']";
    
    private readonly IPageFetcher pageFetcher;

    public EnrollmentSelector(IPageFetcher pageFetcher)
    {
        this.pageFetcher = pageFetcher;
    }

    public async Task<User> SelectAsync(ISession session,
        Enrollment enrollment,
        IEnumerable<Enrollment> enrollments,
        CancellationToken cancellationToken = default)
    {
        var response = await pageFetcher.FetchAndParseWithFormSubmissionAsync(
            AccountPages.EnrollmentSelector,
            enrollment.Data,
            session,
            cancellationToken);

        return response.Url.AbsoluteUri.Contains("discente.jsf")
            ? new User(enrollment, enrollments)
            : throw new ScrapingException(
                $"Unexpected response after submitting enrollment selector form: {response.Url}.");
    }
}