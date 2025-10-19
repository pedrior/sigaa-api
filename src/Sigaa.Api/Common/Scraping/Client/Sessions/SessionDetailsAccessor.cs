using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal sealed class SessionDetailsAccessor : ISessionDetailsAccessor
{
    private readonly IMemoryCache cache;
    private readonly IDataProtectionProvider dataProtectionProvider;

    public SessionDetailsAccessor(IMemoryCache cache, IDataProtectionProvider dataProtectionProvider)
    {
        this.cache = cache;
        this.dataProtectionProvider = dataProtectionProvider;
    }

    public ValueTask<ISessionDetails?> GetSessionDetailsAsync(string sessionId,
        CancellationToken cancellation = default)
    {
        if (!cache.TryGetValue<string?>(sessionId, out var secured) || secured is null)
        {
            return new ValueTask<ISessionDetails?>((ISessionDetails?)null);
        }

        var data = dataProtectionProvider.CreateProtector(sessionId)
            .ToTimeLimitedDataProtector()
            .Unprotect(secured);

        var session = JsonSerializer.Deserialize<Session>(data);
        return new ValueTask<ISessionDetails?>(session);
    }
}