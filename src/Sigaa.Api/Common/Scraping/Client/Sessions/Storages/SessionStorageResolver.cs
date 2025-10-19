using Sigaa.Api.Common.Scraping.Client.Sessions.Cookies;

namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal sealed class SessionStorageResolver : ISessionStorageResolver
{
    private readonly IServiceProvider serviceProvider;

    public SessionStorageResolver(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ISessionStorage Resolve(HttpRequestMessage request)
    {
        var sessionPolicy = request.GetSessionPolicy();
        var requestedNewSessionId = request.GetRequestedNewSessionId();
        var sessionMissingBehavior = request.GetMissingSessionBehavior();

        var sessionStorage = serviceProvider.GetRequiredKeyedService<ISessionStorage>(sessionPolicy);

        sessionStorage.RequestedNewSessionId = requestedNewSessionId;
        sessionStorage.MissingSessionBehavior = sessionMissingBehavior;

        return sessionStorage;
    }
}