using Sigaa.Api.Common.Scraping.Browsing.Sessions;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(SingleEnrollmentHandler))]
public sealed class SingleEnrollmentHandlerTests
{
    private readonly ISession session;
    private readonly IEnrollmentProvider enrollmentProvider;
    private readonly SingleEnrollmentHandler sut;

    public SingleEnrollmentHandlerTests()
    {
        session = A.Fake<ISession>();
        enrollmentProvider = A.Fake<IEnrollmentProvider>();

        sut = new SingleEnrollmentHandler(enrollmentProvider);
    }

    [Fact]
    public void Evaluate_WhenIsStudentPageAndNoEnrollmentLink_ShouldReturnTrue()
    {
        // Arrange
        var page = A.Fake<IDocument>();
        
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com/discente.jsf"));
        A.CallTo(() => page.Query(EnrollmentSelector.EnrollmentSelectorLinkSelector)).Returns(null);

        // Act
        var result = sut.Evaluate(page);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentMatches_ShouldReturnUser()
    {
        // Arrange
        var page = A.Fake<IDocument>();
        
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com"));

        var enrollment = new Enrollment
        {
            Data =
            {
                ["identificador"] = "123"
            }
        };

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(session, CancellationToken.None))
            .Returns([enrollment]);

        // Act
        var user = await sut.HandleAsync(session, page, "123");

        // Assert
        user.Enrollment.Should().Be(enrollment);
        user.Enrollments.Should().HaveCount(1);
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentDoesNotMatch_ShouldThrowInvalidEnrollmentException()
    {
        // Arrange
        var page = A.Fake<IDocument>();

        A.CallTo(() => page.Url).Returns(new Uri("https://example.com"));

        var enrollment = new Enrollment
        {
            Data =
            {
                ["identificador"] = "123"
            }
        };

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(session, CancellationToken.None))
            .Returns([enrollment]);

        // Act
        var act = () => sut.HandleAsync(session, page, "456");

        // Assert
        await act.Should().ThrowAsync<InvalidEnrollmentException>();
    }
}