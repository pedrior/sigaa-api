namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal abstract class SessionStorage : ISessionStorage
{
    public string? RequestedNewSessionId { get; set; }
    
    public virtual MissingSessionBehavior MissingSessionBehavior { get; set; }

    public abstract ValueTask<ISession> GetSessionAsync(CancellationToken cancellation = default);

    public virtual Task SaveSessionAsync(ISession session, CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }
}