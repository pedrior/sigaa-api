using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping.Client;

internal sealed class Fetcher : IFetcher
{
    private readonly HttpClient client;
    private readonly IHtmlParser parser;

    public Fetcher(HttpClient client, IHtmlParser parser, IOptionsSnapshot<FetcherClientOptions> options)
    {
        this.client = client;
        this.parser = parser;

        ConfigureClient(options.Value);
    }

    public IDocumentRequestBuilder FetchDocumentAsync(Uri uri, CancellationToken cancellation = default) =>
        new DocumentRequestBuilder(uri, client, parser, cancellation);

    public IDocumentRequestBuilder FetchDocumentAsync(string uri, CancellationToken cancellation = default) =>
        FetchDocumentAsync(new Uri(uri, UriKind.RelativeOrAbsolute), cancellation);

    private void ConfigureClient(FetcherClientOptions options)
    {
        client.BaseAddress = options.BaseUrl;

        foreach (var header in options.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }
}