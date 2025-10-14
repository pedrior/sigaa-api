using Sigaa.Api.Common.Scraping.Browsing;
using Sigaa.Api.Common.Scraping.Browsing.Sessions;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Common.Scraping.Exceptions;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(EnrollmentSelector))]
public sealed class EnrollmentSelectorTests
{
    private readonly IResourceLoader resourceLoader;
    private readonly EnrollmentSelector sut;
    private readonly ISession session;

    public EnrollmentSelectorTests()
    {
        resourceLoader = A.Fake<IResourceLoader>();
        session = A.Fake<ISession>();
        sut = new EnrollmentSelector(resourceLoader);
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
        var browserRequest = A.Fake<IDocumentRequest>();
    
        A.CallTo(() => successPage.Url).Returns(new Uri("https://example.com/discente.jsf"));
        
        A.CallTo(() => resourceLoader.LoadDocumentAsync(AccountPages.EnrollmentSelector))
            .Returns(browserRequest);

        A.CallTo(() => browserRequest.WithFormData(enrollmentToSelect.Data))
            .Returns(browserRequest);
        
        A.CallTo(() => browserRequest.WithSession(session, A<CancellationToken>.Ignored))
            .Returns(browserRequest);

        A.CallTo(() => browserRequest.GetAwaiter())
            .Returns(Task.FromResult(successPage).GetAwaiter());

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
        var fakeBrowserRequest = A.Fake<IDocumentRequest>();

        A.CallTo(() => failurePage.Url).Returns(new Uri("https://example.com/vinculos.jsf"));

        // --- Refactored Part ---
        A.CallTo(() => resourceLoader.LoadDocumentAsync(A<string>._))
            .Returns(fakeBrowserRequest);

        A.CallTo(() => fakeBrowserRequest.WithFormData(A<Dictionary<string, string>>._))
            .Returns(fakeBrowserRequest);
        
        A.CallTo(() => fakeBrowserRequest.WithSession(A<ISession>._, A<CancellationToken>._))
            .Returns(fakeBrowserRequest);

        A.CallTo(() => fakeBrowserRequest.GetAwaiter())
            .Returns(Task.FromResult(failurePage).GetAwaiter());

        // Act
        var act = () => sut.SelectAsync(session, enrollmentToSelect, new List<Enrollment>());

        // Assert
        await act.Should().ThrowAsync<ScrapingException>()
            .WithMessage(
                "Unexpected response after submitting enrollment selector form: https://example.com/vinculos.jsf.");
    }
}