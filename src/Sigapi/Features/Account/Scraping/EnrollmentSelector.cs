using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Exceptions;

namespace Sigapi.Features.Account.Scraping;

internal sealed class EnrollmentSelector : IEnrollmentSelector
{
    public const string EnrollmentSelectorLinkSelector = "a[href*='vinculos/listar']";
    
    private readonly IResourceLoader resourceLoader;

    public EnrollmentSelector(IResourceLoader resourceLoader)
    {
        this.resourceLoader = resourceLoader;
    }

    public async Task<User> SelectAsync(ISession session,
        Enrollment enrollment,
        IEnumerable<Enrollment> enrollments,
        CancellationToken cancellationToken = default)
    {
        var document = await resourceLoader.LoadDocumentAsync(AccountPages.EnrollmentSelector)
            .WithFormData(enrollment.Data)
            .WithSession(session, cancellationToken);

        return document.Url.AbsoluteUri.Contains("discente.jsf")
            ? new User(enrollment, enrollments)
            : throw new ScrapingException(
                $"Unexpected response after submitting enrollment selector form: {document.Url}.");
    }
}