using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(MultipleEnrollmentHandler))]
public sealed class MultipleEnrollmentHandlerTest
{
    private readonly IEnrollmentProvider enrollmentProvider;
    private readonly IEnrollmentSelector enrollmentSelector;

    private readonly MultipleEnrollmentHandler sut;

    public MultipleEnrollmentHandlerTest()
    {
        enrollmentProvider = A.Fake<IEnrollmentProvider>();
        enrollmentSelector = A.Fake<IEnrollmentSelector>();

        sut = new MultipleEnrollmentHandler(enrollmentProvider, enrollmentSelector);
    }

    [Fact]
    public void Evaluate_WhenUrlIsVinculos_ShouldReturnTrue()
    {
        // Arrange
        var document = A.Fake<IDocument>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com/vinculos.jsf"));

        // Act
        var result = sut.Evaluate(document);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentIsProvidedAndExists_ShouldSelectCorrectEnrollment()
    {
        // Arrange
        var document = A.Fake<IDocument>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com"));

        var enrollment1 = new Enrollment
        {
            Data =
            {
                ["identificador"] = "123"
            }
        };

        var enrollment2 = new Enrollment
        {
            Data =
            {
                ["identificador"] = "456"
            }
        };

        var enrollments = new[]
        {
            enrollment1,
            enrollment2
        };

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(CancellationToken.None))
            .Returns(enrollments);

        // Act
        await sut.HandleAsync(document, "456");

        // Assert
        A.CallTo(() => enrollmentSelector.SelectAsync(enrollment2, enrollments, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentIsNotProvided_ShouldSelectLatestEnrollment()
    {
        // Arrange
        var document = A.Fake<IDocument>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com"));

        var enrollment1 = new Enrollment
        {
            Data =
            {
                ["identificador"] = "123"
            }
        };

        var enrollment2 = new Enrollment
        {
            Data =
            {
                ["identificador"] = "456"
            }
        };

        var enrollments = new[]
        {
            enrollment1, enrollment2
        };

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(CancellationToken.None))
            .Returns(enrollments);

        // Act
        await sut.HandleAsync(document);

        // Assert
        A.CallTo(() => enrollmentSelector.SelectAsync(enrollment2, enrollments, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentDoesNotExist_ShouldThrowInvalidEnrollmentException()
    {
        // Arrange
        var document = A.Fake<IDocument>();

        A.CallTo(() => document.Url).Returns(new Uri("https://example.com"));

        var enrollments = new[]
        {
            new Enrollment
            {
                Data =
                {
                    ["identificador"] = "123"
                }
            }
        };

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(CancellationToken.None))
            .Returns(enrollments);

        // Act
        var act = () => sut.HandleAsync(document, "999");

        // Assert
        await act.Should().ThrowAsync<InvalidEnrollmentException>();
    }
}