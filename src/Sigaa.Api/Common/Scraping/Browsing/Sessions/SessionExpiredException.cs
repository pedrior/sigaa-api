namespace Sigaa.Api.Common.Scraping.Browsing.Sessions;

internal sealed class SessionExpiredException(string message, Exception? inner = null) : Exception(message, inner);