namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal class SessionException : Exception
{
    public SessionException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}