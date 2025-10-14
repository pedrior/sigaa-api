using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Browsing;
using Sigaa.Api.Common.Scraping.Browsing.Sessions;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(EnrollmentProvider))]
public class EnrollmentProviderTests
{
    private readonly IResourceLoader resourceLoader;
    private readonly IScrapingEngine scrapingEngine;
    private readonly EnrollmentProvider enrollmentProvider;
    private readonly ISession session;

    public EnrollmentProviderTests()
    {
        resourceLoader = A.Fake<IResourceLoader>();
        scrapingEngine = A.Fake<IScrapingEngine>();
        session = A.Fake<ISession>();
        enrollmentProvider = new EnrollmentProvider(resourceLoader, scrapingEngine);
    }

    [Fact]
    public async Task ListEnrollmentsAsync_WhenCalled_ShouldFetchScrapeAndMergeEnrollmentData()
    {
        // Arrange
        var page = A.Fake<IDocument>();
        var browserRequest = A.Fake<IDocumentRequest>();
    
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com/vinculos.jsf"));
        A.CallTo(() => resourceLoader.LoadDocumentAsync(AccountPages.EnrollmentSelector))
            .Returns(browserRequest);

        A.CallTo(() => browserRequest.WithSession(session, A<CancellationToken>.Ignored))
            .Returns(browserRequest);
    
        A.CallTo(() => browserRequest.GetAwaiter())
            .Returns(Task.FromResult(page).GetAwaiter());

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