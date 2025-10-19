namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal sealed class SessionExpiredException : SessionException
{
    public SessionExpiredException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}