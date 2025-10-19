using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

namespace Sigaa.Api.Common.Scraping.Client.Sessions.Cookies;

internal sealed class CookieHandler : DelegatingHandler
{
    private readonly ISessionStorageResolver sessionStorageResolver;

    public CookieHandler(ISessionStorageResolver sessionStorageResolver)
    {
        this.sessionStorageResolver = sessionStorageResolver;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellation)
    {
        var storage = sessionStorageResolver.Resolve(request);
        var session = await storage.GetSessionAsync(cancellation);

        session.IncludeCookiesInRequest(request);

        var response = await base.SendAsync(request, cancellation);

        session.ProcessResponseCookies(response);
        await storage.SaveSessionAsync(session, cancellation);

        return response;
    }
}