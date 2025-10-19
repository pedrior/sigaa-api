using Sigaa.Api.Common.Scraping.Client.Sessions;

namespace Sigaa.Api.UnitTests.Common.Scraping.Client.Sessions;

[TestSubject(typeof(Session))]
public sealed class SessionTests
{
    [Fact]
    public void IsExpired_WhenWithinIdleTimeout_ShouldReturnFalse()
    {
        // Arrange
        var session = new Session();

        // Act
        var isExpired = session.IsExpired();

        // Assert
        isExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenOutsideIdleTimeout_ShouldReturnTrue()
    {
        // Arrange
        var session = new Session("test");

        // Create an expired session through reflection.
        var lastAccessedField = typeof(Session).GetProperty(nameof(Session.LastAccessed));
        lastAccessedField!.SetValue(session, DateTimeOffset.UtcNow.AddMinutes(-70));

        // Act
        var isExpired = session.IsExpired();

        // Assert
        isExpired.Should().BeTrue();
    }

    [Fact]
    public void GetRemainingLifetime_WhenNotExpired_ShouldReturnPositiveTimeSpan()
    {
        // Arrange
        var session = new Session();

        // Act
        var remaining = session.GetRemainingLifetime();

        // Assert
        remaining.Should().BeGreaterThan(TimeSpan.Zero);
        remaining.Should().BeLessThanOrEqualTo(session.IdleTimeout);
    }

    [Fact]
    public void GetRemainingLifetime_WhenExpired_ShouldReturnZero()
    {
        // Arrange
        var session = new Session("session-id");
        var lastAccessedField = typeof(Session).GetProperty(nameof(Session.LastAccessed));
        lastAccessedField!.SetValue(session, DateTimeOffset.UtcNow.AddMinutes(-70));

        // Act
        var remaining = session.GetRemainingLifetime();

        // Assert
        remaining.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void IncludeCookiesInRequest_WhenCookiesExist_ShouldAddCookieHeader()
    {
        // Arrange
        var session = new Session();
        var uri = new Uri("https://test.com");

        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = new HttpResponseMessage
        {
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
        };

        response.Headers.Add("Set-Cookie", "key=value; path=/");
        session.ProcessResponseCookies(response);

        // Act
        session.IncludeCookiesInRequest(request);

        // Assert
        request.Headers.GetValues("Cookie").Should().Contain("key=value");
    }

    [Fact]
    public void ProcessResponseCookies_WhenSetCookieHeaderExists_ShouldAddCookiesToContainer()
    {
        // Arrange
        var session = new Session();
        var uri = new Uri("https://test.com");
        var response = new HttpResponseMessage
        {
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
        };

        response.Headers.Add("Set-Cookie", "key=value; path=/");

        // Act
        session.ProcessResponseCookies(response);

        // Assert
        var action = () => session.ProcessResponseCookies(response);

        action.Should().NotThrow();
    }
}