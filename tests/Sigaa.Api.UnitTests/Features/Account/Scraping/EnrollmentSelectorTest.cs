using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Common.Scraping.Exceptions;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(EnrollmentSelector))]
public sealed class EnrollmentSelectorTest
{
    private readonly IFetcher fetcher;
    private readonly EnrollmentSelector sut;

    public EnrollmentSelectorTest()
    {
        fetcher = A.Fake<IFetcher>();
        sut = new EnrollmentSelector(fetcher);
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

        var successDocument = A.Fake<IDocument>();
        var documentRequest = A.Fake<IPersistentDocumentRequestBuilder>();
    
        A.CallTo(() => successDocument.Url).Returns(new Uri("https://example.com/discente.jsf"));
        
        A.CallTo(() => fetcher.FetchDocumentAsync(AccountPages.EnrollmentSelector, A<CancellationToken>.Ignored))
            .Returns(documentRequest);

        A.CallTo(() => documentRequest.WithFormData(enrollmentToSelect.Data))
            .Returns(documentRequest);
        
        A.CallTo(() => documentRequest.WithPersistentSession())
            .Returns(documentRequest);

        A.CallTo(() => documentRequest.GetAwaiter())
            .Returns(Task.FromResult(successDocument).GetAwaiter());

        // Act
        var user = await sut.SelectAsync(enrollmentToSelect, enrollments);

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

        var failureDocument = A.Fake<IDocument>();
        var documentRequest = A.Fake<IPersistentDocumentRequestBuilder>();

        A.CallTo(() => failureDocument.Url).Returns(new Uri("https://example.com/vinculos.jsf"));
        
        A.CallTo(() => fetcher.FetchDocumentAsync(A<string>._, A<CancellationToken>._))
            .Returns(documentRequest);

        A.CallTo(() => documentRequest.WithFormData(A<Dictionary<string, string>>._))
            .Returns(documentRequest);
        
        A.CallTo(() => documentRequest.WithPersistentSession())
            .Returns(documentRequest);

        A.CallTo(() => documentRequest.GetAwaiter())
            .Returns(Task.FromResult(failureDocument).GetAwaiter());

        // Act
        var act = () => sut.SelectAsync(enrollmentToSelect, new List<Enrollment>());

        // Assert
        await act.Should().ThrowAsync<ScrapingException>()
            .WithMessage(
                "Unexpected response after submitting enrollment selector form: https://example.com/vinculos.jsf.");
    }
}