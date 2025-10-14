using Microsoft.IdentityModel.Tokens;

namespace Sigaa.Api.Common.Security.Tokens;

internal sealed class SecurityTokenProvider : ISecurityTokenProvider
{
    private const int TokenExpirationMinutes = 60;

    private readonly TimeProvider time;
    private readonly SecurityTokenOptions options;

    public SecurityTokenProvider(TimeProvider time, IOptions<SecurityTokenOptions> options)
    {
        this.time = time;
        this.options = options.Value;
    }

    public SecurityToken CreateToken(IDictionary<string, object> claims)
    {
        var now = time.GetUtcNow();
        var expiresAt = now.AddMinutes(TokenExpirationMinutes);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = expiresAt.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Issuer = options.Issuer,
            Audience = options.Audience,
            SigningCredentials = signingCredentials,
            Claims = new Dictionary<string, object>(claims)
            {
                [JwtRegisteredClaimNames.Jti] = Guid.CreateVersion7()
            }
        };

        var tokenHandler = new JsonWebTokenHandler
        {
            MapInboundClaims = false,
            SetDefaultTimesOnTokenCreation = false
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new SecurityToken(token, expiresAt);
    }
}