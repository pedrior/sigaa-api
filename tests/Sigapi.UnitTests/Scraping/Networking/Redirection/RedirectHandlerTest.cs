using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Sigapi.Scraping.Networking.Redirection;

namespace Sigapi.UnitTests.Scraping.Networking.Redirection;

[TestSubject(typeof(RedirectHandler))]
public sealed class RedirectHandlerTests
{
    private readonly ILogger<RedirectHandler> logger = A.Fake<ILogger<RedirectHandler>>();

    private (HttpClient client, MockHttpMessageHandler messageHandler) CreateTestClient()
    {
        var redirectHandler = new RedirectHandler(logger)
        {
            InnerHandler = new MockHttpMessageHandler()
        };

        var client = new HttpClient(redirectHandler);
        return (client, (MockHttpMessageHandler)redirectHandler.InnerHandler);
    }

    [Fact]
    public async Task SendAsync_WhenResponseIsNotRedirect_ShouldReturnOriginalResponse()
    {
        // Arrange
        var (client, messageHandler) = CreateTestClient();
        var finalResponse = new HttpResponseMessage(HttpStatusCode.OK);

        messageHandler.NextResponse = finalResponse;

        // Act
        var response = await client.GetAsync("http://example.com");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.RequestMessage!.RequestUri!.ToString().Should().Be("http://example.com/");
        
        messageHandler.NumberOfCalls.Should().Be(1);
    }

    [Fact]
    public async Task SendAsync_WhenSingleRedirectOccurs_ShouldFollowRedirectAndReturnFinalResponse()
    {
        // Arrange
        var (client, messageHandler) = CreateTestClient();
        var redirectResponse = new HttpResponseMessage(HttpStatusCode.MovedPermanently);

        redirectResponse.Headers.Location = new Uri("http://example.com/new");

        messageHandler.NextResponse = redirectResponse;
        messageHandler.FinalResponse = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        var response = await client.GetAsync("http://example.com/old");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        messageHandler.NumberOfCalls.Should().Be(2);
        messageHandler.LastRequest?.RequestUri?.ToString().Should().Be("http://example.com/new");
    }

    [Fact]
    public async Task SendAsync_WhenPostIsRedirectedWith301_ShouldChangeMethodToGetAndClearContent()
    {
        // Arrange
        var (client, messageHandler) = CreateTestClient();
        var redirectResponse = new HttpResponseMessage(HttpStatusCode.MovedPermanently); // 301

        redirectResponse.Headers.Location = new Uri("http://example.com/new");

        messageHandler.NextResponse = redirectResponse;
        messageHandler.FinalResponse = new HttpResponseMessage(HttpStatusCode.OK);

        var initialRequest = new HttpRequestMessage(HttpMethod.Post, "http://example.com/old")
        {
            Content = new StringContent("data")
        };

        // Act
        await client.SendAsync(initialRequest);

        // Assert
        messageHandler.NumberOfCalls.Should().Be(2);
        messageHandler.LastRequest?.Method.Should().Be(HttpMethod.Get);
        messageHandler.LastRequest?.Content.Should().BeNull();
    }

    [Fact]
    public async Task SendAsync_WhenPostIsRedirectedWith307_ShouldPreserveMethodAndContent()
    {
        // Arrange
        var (client, messageHandler) = CreateTestClient();
        var redirectResponse = new HttpResponseMessage(HttpStatusCode.TemporaryRedirect); // 307

        redirectResponse.Headers.Location = new Uri("http://example.com/new");

        messageHandler.NextResponse = redirectResponse;
        messageHandler.FinalResponse = new HttpResponseMessage(HttpStatusCode.OK);

        var initialRequest = new HttpRequestMessage(HttpMethod.Post, "http://example.com/old")
        {
            Content = new StringContent("original data")
        };

        // Act
        await client.SendAsync(initialRequest);

        // Assert
        messageHandler.NumberOfCalls.Should().Be(2);
        messageHandler.LastRequest?.Method.Should().Be(HttpMethod.Post);
        var content = await messageHandler.LastRequest?.Content?.ReadAsStringAsync()!;
        content.Should().Be("original data");
    }

    [Fact]
    public async Task SendAsync_WhenMaxRedirectsExceeded_ShouldThrowHttpRequestException()
    {
        // Arrange
        var (client, messageHandler) = CreateTestClient();
        var redirectResponse = new HttpResponseMessage(HttpStatusCode.MovedPermanently);

        redirectResponse.Headers.Location = new Uri("http://example.com/redirect");

        // This will cause an infinite loop up to the limit
        messageHandler.NextResponse = redirectResponse;
        messageHandler.FinalResponse = redirectResponse;

        // Act
        var act = () => client.GetAsync("http://example.com");

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Maximum number of redirects (10) exceeded at http://example.com/redirect");
    }

    [Fact]
    public async Task SendAsync_WhenRedirecting_ShouldRemoveAuthorizationHeader()
    {
        // Arrange
        var (client, messageHandler) = CreateTestClient();
        var redirectResponse = new HttpResponseMessage(HttpStatusCode.MovedPermanently);
        redirectResponse.Headers.Location = new Uri("http://other.com");

        messageHandler.NextResponse = redirectResponse;
        messageHandler.FinalResponse = new HttpResponseMessage(HttpStatusCode.OK);

        var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token");

        // Act
        await client.SendAsync(request);

        // Assert
        messageHandler.LastRequest?.Headers.Authorization.Should().BeNull();
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage NextResponse { get; set; } = new(HttpStatusCode.OK);

        public HttpResponseMessage FinalResponse { get; set; } = new(HttpStatusCode.OK);

        public int NumberOfCalls { get; private set; }

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            NumberOfCalls++;
            LastRequest = request;

            // Return the redirect response first, then the final one.
            var response = NumberOfCalls is 1
                ? NextResponse
                : FinalResponse;

            // Clone the response to avoid issues with it being disposed.
            var clonedResponse = new HttpResponseMessage(response.StatusCode)
            {
                Content = response.Content,
                RequestMessage = request
            };

            foreach (var header in response.Headers)
            {
                clonedResponse.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return Task.FromResult(clonedResponse);
        }
    }
}