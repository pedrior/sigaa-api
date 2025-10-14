using System.Net;
using Sigaa.Api.Common.Scraping.Browsing.Handlers;
using Sigaa.Api.Common.Scraping.Browsing.Sessions;

namespace Sigaa.Api.UnitTests.Common.Scraping.Networking.Sessions;

[TestSubject(subject: typeof(CookieHandler))]
public sealed class CookieHandlerTests
{
    private static (HttpClient client, MockHttpMessageHandler handler) CreateTestClient()
    {
        var sessionHandler = new CookieHandler
        {
            InnerHandler = new MockHttpMessageHandler()
        };

        return (new HttpClient(handler: sessionHandler), (MockHttpMessageHandler)sessionHandler.InnerHandler);
    }

    [Fact]
    public async Task SendAsync_WhenSessionIsProvided_ShouldAddCookieHeaderToRequest()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        var session = new Session();
        var uri = new Uri(uriString: "http://example.com");

        session.SetCookies(target: uri, cookies: ["key=value"]);

        var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: uri);
        request.Options.Set(key: CookieHandler.SessionKey, value: session);

        // Act
        await client.SendAsync(request: request);

        // Assert
        handler.LastRequest?.Headers.GetValues(name: "Cookie").First().Should().Be(expected: "key=value");
    }

    [Fact]
    public async Task SendAsync_WhenResponseHasSetCookieHeader_ShouldUpdateSession()
    {
        // Arrange
        var (client, messageHandler) = CreateTestClient();
        var session = new Session();

        var uri = new Uri(uriString: "http://example.com");

        var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: uri);
        request.Options.Set(key: CookieHandler.SessionKey, value: session);

        messageHandler.Response.Headers.Add(name: "Set-Cookie", value: "new_key=new_value");

        // Act
        await client.SendAsync(request: request);

        // Assert
        session.GetCookies(target: uri).Should().Be(expected: "new_key=new_value");
    }

    [Fact]
    public async Task SendAsync_WhenSessionIsExpired_ShouldThrowSessionExpiredException()
    {
        // Arrange
        var (client, _) = CreateTestClient();
        var expiredSession = new Session(
            id: "expired",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(minutes: -60),
            expiresAt: DateTimeOffset.UtcNow.AddMinutes(minutes: -1),
            autoRefresh: true,
            cookies: []);

        var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: "http://example.com");
        request.Options.Set(key: CookieHandler.SessionKey, value: expiredSession);

        // Act
        var act = () => client.SendAsync(request: request);

        // Assert
        await act.Should().ThrowAsync<SessionExpiredException>();
    }

    [Fact]
    public async Task SendAsync_WhenAutoRefreshIsTrue_ShouldRefreshSessionLifetimeOnSuccess()
    {
        // Arrange
        var (client, _) = CreateTestClient();
        var session = (Session)new SessionManager(sessionStore: A.Fake<ISessionStore>())
            .CreateSession();
        
        session.AutoRefreshLifetime = true;
        
        var initialExpiry = session.ExpiresAt;

        var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: "http://example.com");
        
        request.Options.Set(key: CookieHandler.SessionKey, value: session);

        // Act
        Thread.Sleep(millisecondsTimeout: 10); // Ensure time progresses
        
        await client.SendAsync(request: request);

        // Assert
        session.ExpiresAt.Should().BeAfter(expected: initialExpiry);
    }
    
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage Response { get; set; } = new(statusCode: HttpStatusCode.OK);
        
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            Response.RequestMessage = request;
            
            return Task.FromResult(result: Response);
        }
    }
}