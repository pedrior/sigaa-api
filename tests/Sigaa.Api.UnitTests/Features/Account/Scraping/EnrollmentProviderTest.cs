using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(EnrollmentProvider))]
public class EnrollmentProviderTest
{
    private readonly IFetcher fetcher;
    private readonly IScrapingEngine scrapingEngine;
    private readonly EnrollmentProvider enrollmentProvider;

    public EnrollmentProviderTest()
    {
        fetcher = A.Fake<IFetcher>();
        scrapingEngine = A.Fake<IScrapingEngine>();
        enrollmentProvider = new EnrollmentProvider(fetcher, scrapingEngine);
    }

    [Fact]
    public async Task ListEnrollmentsAsync_WhenCalled_ShouldFetchScrapeAndMergeEnrollmentData()
    {
        // Arrange
        var document = A.Fake<IDocument>();
        var documentRequest = A.Fake<IPersistentDocumentRequestBuilder>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com/vinculos.jsf"));
        A.CallTo(() => fetcher.FetchDocumentAsync(AccountPages.EnrollmentSelector, A<CancellationToken>.Ignored))
            .Returns(documentRequest);

        A.CallTo(() => documentRequest.WithPersistentSession())
            .Returns(documentRequest);

        A.CallTo(() => documentRequest.GetAwaiter())
            .Returns(Task.FromResult(document).GetAwaiter());

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

        A.CallTo(() => scrapingEngine.Scrape<UserEnrollments>(document)).Returns(enrollments);

        // Act
        var result = (await enrollmentProvider.ListEnrollmentsAsync()).ToList();

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