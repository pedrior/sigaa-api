namespace Sigapi.Features.Account.Exceptions;

internal sealed class InvalidCredentialsException : LoginException
{
    public InvalidCredentialsException(string message = "Invalid username or password.", Exception? inner = null) 
        : base(message, inner)
    {
    }
}