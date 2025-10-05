using Sigapi.Scraping.Document;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Networking;

internal sealed class PageFetcher : IPageFetcher
{
    private readonly IHttpFetcher httpFetcher;
    private readonly IHtmlParser htmlParser;

    public PageFetcher(IHttpFetcher httpFetcher, IHtmlParser htmlParser)
    {
        this.httpFetcher = httpFetcher;
        this.htmlParser = htmlParser;
    }

    public async Task<IDocument> FetchAndParseAsync(string url,
        ISession? session = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpFetcher.FetchAsync(url, session, cancellationToken);
        var document = await htmlParser.ParseAsync(response.html, cancellationToken);

        document.Url = response.url;

        return document;
    }

    public async Task<IDocument> FetchAndParseWithFormSubmissionAsync(string url,
        IReadOnlyDictionary<string, string> data,
        ISession? session = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpFetcher.FetchWithFormSubmissionAsync(url, data, session, cancellationToken);
        var document = await htmlParser.ParseAsync(response.html, cancellationToken);

        document.Url = response.url;

        return document;
    }
}