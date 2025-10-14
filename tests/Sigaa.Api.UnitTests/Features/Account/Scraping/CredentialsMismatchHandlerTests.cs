using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(CredentialsMismatchHandler))]
public sealed class CredentialsMismatchHandlerTests
{
    private readonly CredentialsMismatchHandler sut = new();

    [Fact]
    public void Evaluate_WhenUrlContainsLogon_ShouldReturnTrue()
    {
        // Arrange
        var page = A.Fake<IDocument>();
        
        A.CallTo(() => page.Url).Returns(new Uri("https://sigaa.ufpb.br/sigaa/logon.jsf"));
        
        // Act
        var result = sut.Evaluate(page);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WhenUrlDoesNotContainLogon_ShouldReturnFalse()
    {
        // Arrange
        var page = A.Fake<IDocument>();
        
        A.CallTo(() => page.Url).Returns(new Uri("https://sigaa.ufpb.br/sigaa/somepage.jsf"));

        // Act
        var result = sut.Evaluate(page);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WhenCalled_ShouldThrowInvalidCredentialsException()
    {
        // Arrange
        // Act
        var act = () => sut.HandleAsync(
            A.Dummy<ISession>(),
            A.Dummy<IDocument>(),
            enrollment: null,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}