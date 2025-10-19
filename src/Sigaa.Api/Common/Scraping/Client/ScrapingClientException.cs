namespace Sigaa.Api.Common.Scraping.Client;

internal sealed class ScrapingClientException : Exception
{
    public ScrapingClientException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}