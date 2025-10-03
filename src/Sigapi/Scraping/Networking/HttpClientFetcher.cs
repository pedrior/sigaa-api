using Sigapi.Scraping.Networking.Sessions;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Networking;

internal sealed class HttpClientFetcher : IHttpFetcher
{
    internal const string ClientName = "ScrapingClient";
    
    private readonly IHttpClientFactory clientFactory;

    public HttpClientFetcher(IHttpClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    public async Task<(Uri url, string html)> FetchAsync(string url,
        ISession? session,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await FetchInternalAsync(request, session, cancellationToken);
    }

    public Task<(Uri url, string html)> FetchWithFormSubmissionAsync(string url,
        IReadOnlyDictionary<string, string> data,
        ISession? session,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(data)
        };

        return FetchInternalAsync(request, session, cancellationToken);
    }

    private async Task<(Uri url, string html)> FetchInternalAsync(HttpRequestMessage request,
        ISession? session,
        CancellationToken cancellationToken)
    {
        var client = clientFactory.CreateClient(ClientName);

        // Configure the session for the request.
        if (session is not null)
        {
            request.Options.Set(SessionHandler.SessionKey, session);
        }

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync(cancellationToken);

        // Some fetches may be redirected, so we return the final URL.
        var finalUrl = response.RequestMessage!.RequestUri!;

        return (finalUrl, html);
    }
}