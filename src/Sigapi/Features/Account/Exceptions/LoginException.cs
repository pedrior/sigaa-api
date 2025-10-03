namespace Sigapi.Features.Account.Exceptions;

internal class LoginException(string message, Exception? inner = null) : Exception(message, inner);