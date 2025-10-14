using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Browsing;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping;

internal sealed class EnrollmentProvider : IEnrollmentProvider
{
    private readonly IResourceLoader resourceLoader;
    private readonly IScrapingEngine scrapingEngine;

    public EnrollmentProvider(IResourceLoader resourceLoader, IScrapingEngine scrapingEngine)
    {
        this.resourceLoader = resourceLoader;
        this.scrapingEngine = scrapingEngine;
    }

    public async Task<IEnumerable<Enrollment>> ListEnrollmentsAsync(ISession session,
        CancellationToken cancellationToken = default)
    {
        var document = await resourceLoader.LoadDocumentAsync(AccountPages.EnrollmentSelector)
            .WithSession(session, cancellationToken);

        var enrollments = scrapingEngine.Scrape<UserEnrollments>(document);
        return enrollments.Active
            .Concat(enrollments.Inactive)
            .Select(ApplyCommonEnrollmentData);

        Enrollment ApplyCommonEnrollmentData(Enrollment enrollment)
        {
            var data = enrollment.Data.ToDictionary();
            foreach (var (name, value) in enrollments.Data)
            {
                data[name] = value;
            }

            return new Enrollment
            {
                Data = data
            };
        }
    }
}