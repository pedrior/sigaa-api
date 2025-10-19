namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal interface ISessionDetailsAccessor
{
    ValueTask<ISessionDetails?> GetSessionDetailsAsync(string sessionId, CancellationToken cancellation = default);
}