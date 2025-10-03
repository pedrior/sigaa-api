namespace Sigapi.Features.Account.Exceptions;

internal sealed class InvalidEnrollmentException : LoginException
{
    public InvalidEnrollmentException(string message = "Invalid enrollment number.", Exception? inner = null) 
        : base(message, inner)
    {
    }
}