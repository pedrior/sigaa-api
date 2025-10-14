using System.Security.Claims;
using Sigaa.Api.Common.Security.Tokens;

namespace Sigaa.Api.Common.Security;

internal sealed class UserContext : IUserContext
{
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null or { Identity: null or { IsAuthenticated: false } })
        {
            throw new UnauthorizedAccessException("The user is not authenticated.");
        }

        ReadClaimValues(user);
    }

    public string Username { get; private set; } = string.Empty;

    public string Enrollment { get; private set; } = string.Empty;

    public string[] Enrollments { get; private set; } = [];

    public string SessionId { get; private set; } = string.Empty;

    private void ReadClaimValues(ClaimsPrincipal user)
    {
        Username = user.FindFirstValue(CustomClaimTypes.Username)!;
        Enrollment = user.FindFirstValue(CustomClaimTypes.Enrollment)!;
        SessionId = user.FindFirstValue(JwtRegisteredClaimNames.Sid)!;
        Enrollments = user.FindAll(CustomClaimTypes.Enrollments)
            .Select(c => c.Value)
            .ToArray();
    }
}