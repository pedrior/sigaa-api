using Sigapi.Scraping.Browsing.Sessions;

namespace Sigapi.UnitTests.Scraping.Networking.Sessions;

[TestSubject(typeof(Session))]
public sealed class SessionTests
{
    [Fact]
    public void IsExpired_WhenExpiresAtIsInTheFuture_ShouldBeFalse()
    {
        // Arrange
        var session = new Session();

        // Act & Assert
        session.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenExpiresAtIsInThePast_ShouldBeTrue()
    {
        // Arrange
        var expiredSession = new Session(
            id: "expired",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-60),
            expiresAt: DateTimeOffset.UtcNow.AddMinutes(-1),
            autoRefresh: true,
            cookies: []);

        // Act & Assert
        expiredSession.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void SetExpirationFromNow_WhenCalled_ShouldUpdateExpiresAt()
    {
        // Arrange
        var session = (Session)new SessionManager(A.Fake<ISessionStore>())
            .CreateSession();
        
        var initialExpiry = session.ExpiresAt;
            
        // Act
        Thread.Sleep(10); // Ensure time progresses
        session.SetExpirationFromNow();
        
        var newExpiry = session.ExpiresAt;

        // Assert
        newExpiry.Should().BeAfter(initialExpiry);
    }
}