using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping;

internal sealed class EnrollmentProvider : IEnrollmentProvider
{
    private readonly IFetcher fetcher;
    private readonly IScrapingEngine scrapingEngine;

    public EnrollmentProvider(IFetcher fetcher, IScrapingEngine scrapingEngine)
    {
        this.fetcher = fetcher;
        this.scrapingEngine = scrapingEngine;
    }

    public async Task<IEnumerable<Enrollment>> ListEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        var document = await fetcher.FetchDocumentAsync(AccountPages.EnrollmentSelector, cancellationToken)
            .WithPersistentSession();

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