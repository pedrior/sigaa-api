using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Networking;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Features.Account.Scraping;

internal sealed class EnrollmentProvider : IEnrollmentProvider
{
    private readonly IPageFetcher pageFetcher;
    private readonly IScrapingEngine scrapingEngine;

    public EnrollmentProvider(IPageFetcher pageFetcher, IScrapingEngine scrapingEngine)
    {
        this.pageFetcher = pageFetcher;
        this.scrapingEngine = scrapingEngine;
    }

    public async Task<IEnumerable<Enrollment>> ListEnrollmentsAsync(ISession session,
        CancellationToken cancellationToken = default)
    {
        var page = await pageFetcher.FetchAndParseAsync(
            AccountPages.EnrollmentSelector,
            session,
            cancellationToken);

        var enrollments = scrapingEngine.Scrape<UserEnrollments>(page);
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