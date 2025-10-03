using Sigapi.Features.Account.Exceptions;
using Sigapi.Features.Account.Models;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(MultipleEnrollmentHandler))]
public sealed class MultipleEnrollmentHandlerTests
{
    private readonly IEnrollmentProvider enrollmentProvider;
    private readonly IEnrollmentSelector enrollmentSelector;
    private readonly ISession session;

    private readonly MultipleEnrollmentHandler sut;

    public MultipleEnrollmentHandlerTests()
    {
        enrollmentProvider = A.Fake<IEnrollmentProvider>();
        enrollmentSelector = A.Fake<IEnrollmentSelector>();
        session = A.Fake<ISession>();

        sut = new MultipleEnrollmentHandler(enrollmentProvider, enrollmentSelector);
    }

    [Fact]
    public void Evaluate_WhenUrlIsVinculos_ShouldReturnTrue()
    {
        // Arrange
        var page = A.Fake<IDocument>();

        A.CallTo(() => page.Session).Returns(session);
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com/vinculos.jsf"));

        // Act
        var result = sut.Evaluate(page);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentIsProvidedAndExists_ShouldSelectCorrectEnrollment()
    {
        // Arrange
        var page = A.Fake<IDocument>();

        A.CallTo(() => page.Session).Returns(session);
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com"));

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

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(session, CancellationToken.None))
            .Returns(enrollments);

        // Act
        await sut.HandleAsync(page, "456");

        // Assert
        A.CallTo(() => enrollmentSelector.SelectAsync(session, enrollment2, enrollments, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentIsNotProvided_ShouldSelectLatestEnrollment()
    {
        // Arrange
        var page = A.Fake<IDocument>();

        A.CallTo(() => page.Session).Returns(session);
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com"));

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

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(session, CancellationToken.None))
            .Returns(enrollments);

        // Act
        await sut.HandleAsync(page);

        // Assert
        A.CallTo(() => enrollmentSelector.SelectAsync(session, enrollment2, enrollments, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WhenEnrollmentDoesNotExist_ShouldThrowInvalidEnrollmentException()
    {
        // Arrange
        var page = A.Fake<IDocument>();

        A.CallTo(() => page.Session).Returns(session);
        A.CallTo(() => page.Url).Returns(new Uri("https://example.com"));

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

        A.CallTo(() => enrollmentProvider.ListEnrollmentsAsync(session, CancellationToken.None))
            .Returns(enrollments);

        // Act
        var act = () => sut.HandleAsync(page, "999");

        // Assert
        await act.Should().ThrowAsync<InvalidEnrollmentException>();
    }
}