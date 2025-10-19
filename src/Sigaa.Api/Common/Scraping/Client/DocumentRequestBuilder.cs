using System.Runtime.CompilerServices;
using Sigaa.Api.Common.Scraping.Client.Sessions;
using Sigaa.Api.Common.Scraping.Client.Sessions.Cookies;
using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping.Client;

internal class DocumentRequestBuilder : IDocumentRequestBuilder
{
    private readonly Uri uri;
    private readonly HttpClient client;
    private readonly IHtmlParser parser;
    private readonly CancellationToken cancellation;

    private readonly HttpRequestMessage request;

    internal DocumentRequestBuilder(HttpClient client,
        HttpRequestMessage request,
        IHtmlParser parser,
        CancellationToken cancellation)
    {
        this.client = client;
        this.request = request;
        this.parser = parser;
        this.cancellation = cancellation;

        uri = request.RequestUri!;
    }

    internal DocumentRequestBuilder(Uri uri,
        HttpClient client,
        IHtmlParser parser,
        CancellationToken cancellation)
        : this(
            client,
            new HttpRequestMessage(HttpMethod.Get, uri),
            parser,
            cancellation)
    {
        request.SetSessionPolicy(SessionPolicy.Transient);
        request.SetMissingSessionBehavior(MissingSessionBehavior.Create);
    }

    public IDocumentRequestBuilder WithEphemeralSession()
    {
        request.SetSessionPolicy(SessionPolicy.Ephemeral);
        request.SetMissingSessionBehavior(MissingSessionBehavior.Create);

        return this;
    }

    public IPersistentDocumentRequestBuilder WithPersistentSession()
    {
        request.SetSessionPolicy(SessionPolicy.Persistent);
        request.SetMissingSessionBehavior(MissingSessionBehavior.Throw);

        return new PersistentDocumentRequestBuilder(client, request, parser, cancellation);
    }

    public IDocumentRequestBuilder WithFormData(IDictionary<string, string> data)
    {
        request.Method = HttpMethod.Post;
        request.Content = new FormUrlEncodedContent(data);

        return this;
    }

    public TaskAwaiter<IDocument> GetAwaiter() => ExecuteAsync().GetAwaiter();

    public Task<IDocument> AsTask() => ExecuteAsync();

    private async Task<IDocument> ExecuteAsync()
    {
        string content;
        Uri absoluteUri;
        HttpResponseMessage? response = null;

        try
        {
            response = await client.SendAsync(request, cancellation);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync(cancellation);
            absoluteUri = response.RequestMessage!.RequestUri!;
        }
        catch (HttpRequestException ex)
        {
            throw new ScrapingClientException($"A network error occurred while requesting '{uri}'.", ex);
        }
        finally
        {
            response?.Dispose();
        }

        var document = await parser.ParseAsync(content, cancellation);
        document.Url = absoluteUri;

        return document;
    }
}