using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace Sigaa.Api.Common.Scraping.Browsing.Sessions;

internal sealed class SessionStore : ISessionStore
{
    private readonly IMemoryCache cache;
    private readonly IDataProtectionProvider dataProtectionProvider;

    public SessionStore(IMemoryCache cache, IDataProtectionProvider dataProtectionProvider)
    {
        this.cache = cache;
        this.dataProtectionProvider = dataProtectionProvider;
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new SessionSerializer()
        }
    };

    public Task SaveAsync(ISession session, CancellationToken _)
    {
        var sessionExpiresAt = session.ExpiresAt - DateTimeOffset.UtcNow;
        var payload = Protect(session.Id, JsonSerializer.Serialize(session as Session, SerializerOptions));

        cache.Set(session.Id, payload, sessionExpiresAt);
        return Task.CompletedTask;
    }

    public Task RevokeAsync(string sessionId, CancellationToken _)
    {
        cache.Remove(sessionId);
        return Task.CompletedTask;
    }

    public Task<ISession?> LoadAsync(string sessionId, CancellationToken _)
    {
        var payload = cache.Get<string>(sessionId);
        if (payload is null)
        {
            return Task.FromResult<ISession?>(null);
        }

        try
        {
            var json = Unprotect(sessionId, payload);
            var session = JsonSerializer.Deserialize<Session>(json, SerializerOptions);

            return Task.FromResult<ISession?>(session);
        }
        catch (CryptographicException)
        {
            return Task.FromResult<ISession?>(null);
        }
    }

    private string Protect(string sessionId, string data) => CreateDataProtector(sessionId).Protect(data);

    private string Unprotect(string sessionId, string payload) => CreateDataProtector(sessionId).Unprotect(payload);

    private IDataProtector CreateDataProtector(string sessionId) => dataProtectionProvider.CreateProtector(sessionId);
}