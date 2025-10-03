using Sigapi.Features.Account.Models;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Networking;
using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(EnrollmentProvider))]
public class EnrollmentProviderTests
{
    private readonly IPageFetcher pageFetcher;
    private readonly IScrapingEngine scrapingEngine;
    private readonly EnrollmentProvider enrollmentProvider;
    private readonly ISession session;

    public EnrollmentProviderTests()
    {
        pageFetcher = A.Fake<IPageFetcher>();
        scrapingEngine = A.Fake<IScrapingEngine>();
        session = A.Fake<ISession>();
        enrollmentProvider = new EnrollmentProvider(pageFetcher, scrapingEngine);
    }

    [Fact]
    public async Task ListEnrollmentsAsync_WhenCalled_ShouldFetchScrapeAndMergeEnrollmentData()
    {
        // Arrange
        var page = A.Fake<IDocument>();

        A.CallTo(() => page.Session).Returns(session);
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com/vinculos.jsf"));

        A.CallTo(() => pageFetcher.FetchAndParseAsync(
                AccountPages.EnrollmentSelector,
                session,
                A<CancellationToken>._))
            .Returns(page);

        var enrollments = new UserEnrollments
        {
            Data = new Dictionary<string, string>
            {
                { "form:j_id_hidden", "some_value" }
            },
            Active =
            [
                new Enrollment
                {
                    Data = new Dictionary<string, string>
                    {
                        {
                            "identificador", "123"
                        }
                    }
                }
            ],
            Inactive =
            [
                new Enrollment
                {
                    Data = new Dictionary<string, string>
                    {
                        {
                            "identificador", "456"
                        }
                    }
                }
            ]
        };

        A.CallTo(() => scrapingEngine.Scrape<UserEnrollments>(page)).Returns(enrollments);

        // Act
        var result = (await enrollmentProvider.ListEnrollmentsAsync(session)).ToList();

        // Assert
        result.Should().HaveCount(2);

        var activeEnrollment = result.Should()
            .ContainSingle(e => e.Number == "123").Subject;

        var inactiveEnrollment = result.Should()
            .ContainSingle(e => e.Number == "456").Subject;

        activeEnrollment.Data.Should().ContainKey("form:j_id_hidden");
        activeEnrollment.Data.Should().ContainKey("identificador");

        inactiveEnrollment.Data.Should().ContainKey("form:j_id_hidden");
    }
}