using System.Runtime.CompilerServices;
using Sigapi.Scraping.Browsing.Handlers;
using Sigapi.Scraping.Browsing.Sessions;
using Sigapi.Scraping.Browsing.Sessions.Strategies;
using Sigapi.Scraping.Document;

namespace Sigapi.Scraping.Browsing;

internal sealed class DocumentRequest : IDocumentRequest
{
    private readonly string url;
    private readonly HttpClient httpClient;
    private readonly IHtmlParser htmlParser;
    private readonly ISessionStrategyProvider sessionStrategyProvider;
    private readonly ISessionManager sessionManager;

    private ISessionStrategy sessionStrategy;
    private IReadOnlyDictionary<string, string>? formData;
    private CancellationToken requestCancellationToken = CancellationToken.None;

    public DocumentRequest(string url,
        HttpClient httpClient,
        IHtmlParser htmlParser,
        ISessionManager sessionManager,
        ISessionStrategyProvider sessionStrategyProvider)
    {
        this.url = url;
        this.httpClient = httpClient;
        this.htmlParser = htmlParser;
        this.sessionStrategyProvider = sessionStrategyProvider;
        this.sessionManager = sessionManager;

        // Default to anonymous session if not otherwise configured.
        sessionStrategy = this.sessionStrategyProvider.GetStrategy<AnonymousSessionStrategy>();
    }

    public IDocumentRequest WithFormData(IReadOnlyDictionary<string, string> data)
    {
        formData = data;
        return this;
    }

    public IDocumentRequest WithSession(ISession session, CancellationToken cancellationToken = default)
    {
        sessionStrategy = new CustomSessionStrategy(session);
        requestCancellationToken = cancellationToken;

        return this;
    }

    public IDocumentRequest WithUserSession(CancellationToken cancellationToken = default)
    {
        sessionStrategy = sessionStrategyProvider.GetStrategy<UserSessionStrategy>();
        requestCancellationToken = cancellationToken;

        return this;
    }

    public IDocumentRequest WithContextualSession(CancellationToken cancellationToken = default)
    {
        sessionStrategy = sessionStrategyProvider.GetStrategy<ContextualSessionStrategy>();
        requestCancellationToken = cancellationToken;

        return this;
    }

    public IDocumentRequest WithAnonymousSession(CancellationToken cancellationToken = default)
    {
        sessionStrategy = sessionStrategyProvider.GetStrategy<AnonymousSessionStrategy>();
        requestCancellationToken = cancellationToken;

        return this;
    }

    public TaskAwaiter<IDocument> GetAwaiter() => FetchAsync().GetAwaiter();

    public Task<IDocument> AsTask() => FetchAsync();

    private async Task<IDocument> FetchAsync()
    {
        requestCancellationToken.ThrowIfCancellationRequested();

        var request = CreateRequestMessage();
        var session = await InitializeSessionAsync(request);

        var response = await httpClient.SendAsync(request, requestCancellationToken);
        response.EnsureSuccessStatusCode();

        await TrySaveSessionDataAsync(session);

        var content = await response.Content.ReadAsStringAsync(requestCancellationToken);
        var document = await htmlParser.ParseAsync(content, requestCancellationToken);

        // Set the final URL.
        document.Url = response.RequestMessage!.RequestUri!;

        return document;
    }

    private HttpRequestMessage CreateRequestMessage()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (formData is not null)
        {
            request.Method = HttpMethod.Post;
            request.Content = new FormUrlEncodedContent(formData);
        }

        return request;
    }

    private async Task<ISession> InitializeSessionAsync(HttpRequestMessage request)
    {
        var session = await sessionStrategy.GetSessionAsync(requestCancellationToken);
        request.Options.Set(CookieHandler.SessionKey, session);
        return session;
    }

    private async Task TrySaveSessionDataAsync(ISession session)
    {
        if (sessionStrategy is not AnonymousSessionStrategy)
        {
            await sessionManager.SaveSessionAsync(session, requestCancellationToken);
        }
    }
}