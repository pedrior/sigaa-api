namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal interface ISessionStorage
{
    string? RequestedNewSessionId { get; set; }

    MissingSessionBehavior MissingSessionBehavior { get; set; }

    ValueTask<ISession> GetSessionAsync(CancellationToken cancellation = default);

    Task SaveSessionAsync(ISession session, CancellationToken cancellation = default);
}