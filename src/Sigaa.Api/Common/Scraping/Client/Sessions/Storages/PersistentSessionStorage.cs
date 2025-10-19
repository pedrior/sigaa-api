using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal sealed class PersistentSessionStorage : SessionStorage
{
    internal const string SessionIdClaimType = "sid";
    
    private readonly IMemoryCache cache;
    private readonly IDataProtectionProvider dataProtectionProvider;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<PersistentSessionStorage> logger;

    public PersistentSessionStorage(IMemoryCache cache,
        IDataProtectionProvider dataProtectionProvider,
        IHttpContextAccessor httpContextAccessor,
        ILogger<PersistentSessionStorage> logger)
    {
        this.cache = cache;
        this.dataProtectionProvider = dataProtectionProvider;
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    public override MissingSessionBehavior MissingSessionBehavior { get; set; } = MissingSessionBehavior.Throw;

    private string SessionId
    {
        get
        {
            if (string.IsNullOrEmpty(field))
            {
                field = ResolveSessionId();
            }

            return field;
        }
    } = string.Empty;

    public override ValueTask<ISession> GetSessionAsync(CancellationToken cancellation = default)
    {
        var sessionId = SessionId;
        if (!cache.TryGetValue<string>(sessionId, out var data))
        {
            // Attempts to create a new session.
            logger.LogInformation(
                "Creating new persistent session for HTTP request {TraceIdentifier}",
                httpContextAccessor.HttpContext?.TraceIdentifier);

            return MissingSessionBehavior is MissingSessionBehavior.Throw
                ? throw new SessionException("Session not found.")
                : new ValueTask<ISession>(new Session(sessionId));
        }

        var session = Deserialize(sessionId, data!);
        return new ValueTask<ISession>(session);
    }

    public override Task SaveSessionAsync(ISession session, CancellationToken cancellation = default)
    {
        var sessionId = SessionId;
        var lifetime = session.GetRemainingLifetime();
        var data = Serialize(sessionId, (Session)session, lifetime);

        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(lifetime);

        cache.Set(sessionId, data, options);

        return Task.CompletedTask;
    }

    private string Serialize(string sessionId, Session session, TimeSpan lifetime)
    {
        var plaintext = JsonSerializer.Serialize(session);
        var data = dataProtectionProvider.CreateProtector(sessionId)
            .ToTimeLimitedDataProtector()
            .Protect(plaintext, lifetime);

        return data;
    }

    private Session Deserialize(string sessionId, string data)
    {
        string plaintext;
        try
        {
            plaintext = dataProtectionProvider.CreateProtector(sessionId)
                .ToTimeLimitedDataProtector()
                .Unprotect(data);
        }
        catch (CryptographicException ex)
        {
            throw new SessionException("Failed to decrypt session.", ex);
        }

        var session = JsonSerializer.Deserialize<Session>(plaintext)
                      ?? throw new SessionException("Failed to deserialize session.");

        return session;
    }

    private string ResolveSessionId() => (MissingSessionBehavior, RequestedNewSessionId) switch
    {
        (MissingSessionBehavior.Throw, null) => GetSessionIdFromHttpContext(),
        (MissingSessionBehavior.Create, not null) => RequestedNewSessionId,
        _ => throw new InvalidOperationException("Invalid session configuration.")
    };

    private string GetSessionIdFromHttpContext()
    {
        var sessionId = PersistentSessionStorageHelper.GetSessionId(httpContextAccessor.HttpContext);
        return sessionId ?? throw new UnauthorizedAccessException("Session ID not found in HTTP context.");
    }
}