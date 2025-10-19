namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal interface ISessionRevoker
{
    Task RevokeAsync(CancellationToken cancellation = default);
}