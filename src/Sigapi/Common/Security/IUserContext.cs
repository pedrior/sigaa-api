namespace Sigapi.Common.Security;

internal interface IUserContext
{
    string Username { get; }

    string Enrollment { get; }
    
    string[] Enrollments { get; }
    
    string SessionId { get; }
}