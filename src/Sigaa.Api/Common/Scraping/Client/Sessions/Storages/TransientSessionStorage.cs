namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal sealed class TransientSessionStorage : SessionStorage
{
    public override ValueTask<ISession> GetSessionAsync(CancellationToken cancellation = default)
    {
        return new ValueTask<ISession>(new Session());
    }
}