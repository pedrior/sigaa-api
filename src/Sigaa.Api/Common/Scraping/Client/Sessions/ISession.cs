namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal interface ISession : ISessionDetails
{
    void IncludeCookiesInRequest(HttpRequestMessage request);
    
    void ProcessResponseCookies(HttpResponseMessage response);
}