namespace Sigaa.Api.Common.Security;

internal interface IUserIdentity
{
    string Username { get; }

    string Enrollment { get; }
    
    string[] Enrollments { get; }
}