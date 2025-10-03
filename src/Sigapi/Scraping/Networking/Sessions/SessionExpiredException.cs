namespace Sigapi.Scraping.Networking.Sessions;

internal sealed class SessionExpiredException(string message, Exception? inner = null) : Exception(message, inner);