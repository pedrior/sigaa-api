using Sigaa.Api.Common.Scraping.Client.Sessions.Cookies;
using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping.Client;

internal sealed class PersistentDocumentRequestBuilder : DocumentRequestBuilder, IPersistentDocumentRequestBuilder
{
    private readonly HttpRequestMessage request;

    internal PersistentDocumentRequestBuilder(HttpClient client,
        HttpRequestMessage request,
        IHtmlParser parser,
        CancellationToken cancellation) : base(client, request, parser, cancellation)
    {
        this.request = request;
    }

    public IPersistentDocumentRequestBuilder AllowSessionCreation(string requestedSessionId)
    {
        request.SetRequestedNewSessionId(requestedSessionId);
        request.SetMissingSessionBehavior(MissingSessionBehavior.Create);

        return this;
    }
}