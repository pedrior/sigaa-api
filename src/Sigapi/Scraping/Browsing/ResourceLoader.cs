using Sigapi.Scraping.Browsing.Sessions;
using Sigapi.Scraping.Browsing.Sessions.Strategies;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Browsing;

internal sealed class ResourceLoader : IResourceLoader
{
    private readonly HttpClient httpClient;
    private readonly IHtmlParser htmlParser;
    private readonly ISessionManager sessionManager;
    private readonly ISessionStrategyProvider sessionStrategyProvider;

    public ResourceLoader(HttpClient httpClient,
        IHtmlParser htmlParser,
        ISessionManager sessionManager,
        ISessionStrategyProvider sessionStrategyProvider)
    {
        this.httpClient = httpClient;
        this.htmlParser = htmlParser;
        this.sessionManager = sessionManager;
        this.sessionStrategyProvider = sessionStrategyProvider;
    }

    public IDocumentRequest LoadDocumentAsync(string url)
    {
        return new DocumentRequest(
            url,
            httpClient,
            htmlParser,
            sessionManager,
            sessionStrategyProvider);
    }
}