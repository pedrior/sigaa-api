using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.UnitTests.Scraping.Networking.Sessions;

[TestSubject(typeof(SessionManager))]
public sealed class SessionManagerTests
{
    private readonly ISessionStore sessionStore;
    private readonly SessionManager sessionManager;

    public SessionManagerTests()
    {
        sessionStore = A.Fake<ISessionStore>();
        sessionManager = new SessionManager(sessionStore);
    }

    [Fact]
    public void CreateSession_WhenCalled_ReturnsNewSession()
    {
        // Arrange & Act
        var session = sessionManager.CreateSession();

        // Assert
        session.Should().NotBeNull();
        session.IsExpired.Should().BeFalse();
    }

    [Fact]
    public async Task LoadSessionAsync_WhenSessionExists_ReturnsSession()
    {
        // Arrange
        var expectedSession = new Session();
        A.CallTo(() => sessionStore.LoadAsync("exists", A<CancellationToken>._)).Returns(expectedSession);

        // Act
        var session = await sessionManager.LoadSessionAsync("exists");

        // Assert
        session.Should().Be(expectedSession);
    }

    [Fact]
    public async Task LoadSessionAsync_WhenSessionDoesNotExist_ThrowsSessionExpiredException()
    {
        // Arrange
        A.CallTo(() => sessionStore.LoadAsync("missing", A<CancellationToken>._)).Returns<ISession?>(null);

        // Act
        var act = () => sessionManager.LoadSessionAsync("missing");

        // Assert
        await act.Should().ThrowAsync<SessionExpiredException>();
    }

    [Fact]
    public async Task SaveSessionAsync_WhenCalled_SavesSessionToStore()
    {
        // Arrange
        var session = new Session();

        // Act
        await sessionManager.SaveSessionAsync(session);

        // Assert
        A.CallTo(() => sessionStore.SaveAsync(session, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RevokeSessionAsync_WhenCalled_RevokesSessionFromStore()
    {
        // Arrange
        const string sessionId = "to-revoke";

        // Act
        await sessionManager.RevokeSessionAsync(sessionId);

        // Assert
        A.CallTo(() => sessionStore.RevokeAsync(sessionId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ValidateSessionAsync_WhenSessionIsValid_ReturnsTrue()
    {
        // Arrange
        var validSession = new Session();
        A.CallTo(() => sessionStore.LoadAsync("valid", A<CancellationToken>._)).Returns(validSession);

        // Act
        var isValid = await sessionManager.ValidateSessionAsync("valid");

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateSessionAsync_WhenSessionIsExpired_ReturnsFalse()
    {
        // Arrange
        var expiredSession = new Session(
            id: "expired",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-10),
            expiresAt: DateTimeOffset.UtcNow.AddMinutes(-1),
            autoRefresh: true,
            cookies: []);

        A.CallTo(() => sessionStore.LoadAsync("expired", A<CancellationToken>._)).Returns(expiredSession);

        // Act
        var isValid = await sessionManager.ValidateSessionAsync("expired");

        // Assert
        isValid.Should().BeFalse();
    }
}