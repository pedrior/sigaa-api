using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.UnitTests.Features.Account.Scraping;

[TestSubject(typeof(CredentialsMismatchHandler))]
public sealed class CredentialsMismatchHandlerTest
{
    private readonly CredentialsMismatchHandler sut = new();

    [Fact]
    public void Evaluate_WhenUrlContainsLogon_ShouldReturnTrue()
    {
        // Arrange
        var document = A.Fake<IDocument>();
        
        A.CallTo(() => document.Url).Returns(new Uri("https://sigaa.ufpb.br/sigaa/logon.jsf"));
        
        // Act
        var result = sut.Evaluate(document);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WhenUrlDoesNotContainLogon_ShouldReturnFalse()
    {
        // Arrange
        var document = A.Fake<IDocument>();
        
        A.CallTo(() => document.Url).Returns(new Uri("https://sigaa.ufpb.br/sigaa/somepage.jsf"));

        // Act
        var result = sut.Evaluate(document);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WhenCalled_ShouldThrowInvalidCredentialsException()
    {
        // Arrange
        // Act
        var act = () => sut.HandleAsync(
            A.Dummy<IDocument>(),
            enrollment: null,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}