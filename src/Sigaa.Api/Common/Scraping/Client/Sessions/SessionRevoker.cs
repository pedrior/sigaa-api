using Microsoft.Extensions.Caching.Memory;
using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal sealed class SessionRevoker : ISessionRevoker
{
    private readonly IMemoryCache cache;
    private readonly IHttpContextAccessor httpContextAccessor;

    public SessionRevoker(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
    {
        this.cache = cache;
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task RevokeAsync(CancellationToken cancellation = default)
    {
        var sessionId = PersistentSessionStorageHelper.GetSessionId(httpContextAccessor.HttpContext);
        if (sessionId is not null)
        {
            cache.Remove(sessionId);
        }

        return Task.CompletedTask;
    }
}