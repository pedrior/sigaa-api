namespace Sigapi.Features.Account.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class UserEnrollments
{
    public IEnumerable<Enrollment> Active { get; set; } = [];

    public IEnumerable<Enrollment> Inactive { get; set; } = [];
    
    public Dictionary<string, string> Data { get; set; } = new();
}