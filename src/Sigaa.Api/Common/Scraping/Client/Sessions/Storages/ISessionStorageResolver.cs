namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal interface ISessionStorageResolver
{
    ISessionStorage Resolve(HttpRequestMessage request);
}