using Sigapi.Features.Account.Models;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Exceptions;
using Sigapi.Scraping.Networking;
using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(EnrollmentSelector))]
public sealed class EnrollmentSelectorTests
{
    private readonly IPageFetcher pageFetcher;
    private readonly EnrollmentSelector sut;
    private readonly ISession session;

    public EnrollmentSelectorTests()
    {
        pageFetcher = A.Fake<IPageFetcher>();
        session = A.Fake<ISession>();
        sut = new EnrollmentSelector(pageFetcher);
    }

    [Fact]
    public async Task SelectAsync_WhenResponseIsStudentPage_ShouldReturnUser()
    {
        // Arrange
        var enrollmentToSelect = new Enrollment
        {
            Data = new Dictionary<string, string>
            {
                { "identificador", "123" }
            }
        };

        var enrollments = new[]
        {
            enrollmentToSelect
        };

        var successPage = A.Fake<IDocument>();

        A.CallTo(() => successPage.Session).Returns(session);
        A.CallTo(() => successPage.Url).Returns(new Uri("https://example.com/discente.jsf"));

        A.CallTo(() => pageFetcher.FetchAndParseWithFormSubmissionAsync(
                AccountPages.EnrollmentSelector,
                enrollmentToSelect.Data,
                session,
                A<CancellationToken>._))
            .Returns(successPage);

        // Act
        var user = await sut.SelectAsync(session, enrollmentToSelect, enrollments);

        // Assert
        user.Should().NotBeNull();
        user.Enrollment.Should().Be(enrollmentToSelect);
        user.Enrollments.Should().BeEquivalentTo(enrollments);
    }

    [Fact]
    public async Task SelectAsync_WhenResponseIsNotStudentPage_ShouldThrowScrapingException()
    {
        // Arrange
        var enrollmentToSelect = new Enrollment
        {
            Data = new Dictionary<string, string>()
        };

        var failurePage = A.Fake<IDocument>();

        A.CallTo(() => failurePage.Session).Returns(session);
        A.CallTo(() => failurePage.Url).Returns(new Uri("https://example.com/vinculos.jsf"));

        A.CallTo(() => pageFetcher.FetchAndParseWithFormSubmissionAsync(
                A<string>._,
                A<Dictionary<string, string>>._,
                A<ISession>._,
                A<CancellationToken>._))
            .Returns(failurePage);

        // Act
        var act = () => sut.SelectAsync(session, enrollmentToSelect, new List<Enrollment>());

        // Assert
        await act.Should().ThrowAsync<ScrapingException>()
            .WithMessage(
                "Unexpected response after submitting enrollment selector form: https://example.com/vinculos.jsf.");
    }
}