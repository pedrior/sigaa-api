namespace Sigaa.Api.Features.Account.Models;

internal sealed class User(Enrollment enrollment, IEnumerable<Enrollment> enrollments)
{
    public User(Enrollment enrollment) : this(enrollment, [enrollment])
    {
    }
    
    public Enrollment Enrollment => enrollment;

    public IEnumerable<Enrollment> Enrollments => enrollments;
}