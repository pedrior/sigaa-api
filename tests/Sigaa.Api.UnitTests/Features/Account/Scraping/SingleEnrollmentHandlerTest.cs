using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(SingleEnrollmentHandler))]
public sealed class SingleEnrollmentHandlerTest
{
    private readonly IEnrollmentProvider enrollmentProvider;
    private readonly SingleEnrollmentHandler sut;

    public SingleEnrollmentHandlerTest()
    {
        enrollmentProvider = A.Fake<IEnrollmentProvider>();

        sut = new SingleEnrollmentHandler(enrollmentProvider);
    }

    [Fact]
    public void Evaluate_WhenIsStudentPageAndNoEnrollmentLink_ShouldReturnTrue()
    {
        // Arrange
        var document = A.Fake<IDocument>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com/discente.jsf"));
        A.CallTo(() => document.Query(EnrollmentSelector.EnrollmentSelectorLinkSelector)).Returns(null);

        // Act
        var result = sut.Evaluate(document);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentMatches_ShouldReturnUser()
    {
        // Arrange
        var document = A.Fake<IDocument>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com"));

        var enrollment = new Enrollment
        {
            Data =
            {
                ["identificador"] = "123"
            }
        };

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(CancellationToken.None))
            .Returns([enrollment]);

        // Act
        var user = await sut.HandleAsync(document, "123");

        // Assert
        user.Enrollment.Should().Be(enrollment);
        user.Enrollments.Should().HaveCount(1);
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentDoesNotMatch_ShouldThrowInvalidEnrollmentException()
    {
        // Arrange
        var document = A.Fake<IDocument>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com"));

        var enrollment = new Enrollment
        {
            Data =
            {
                ["identificador"] = "123"
            }
        };

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(CancellationToken.None))
            .Returns([enrollment]);

        // Act
        var act = () => sut.HandleAsync(document, "456");

        // Assert
        await act.Should().ThrowAsync<InvalidEnrollmentException>();
    }
}