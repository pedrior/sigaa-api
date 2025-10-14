using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sigaa.Api.Common.Scraping.Browsing.Sessions;
using Sigaa.Api.Common.Security.Tokens;

namespace Sigaa.Api.Common.Security;

internal static class DependencyInjectionServices
{
    public static WebApplicationBuilder AddSecurityServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDataProtection();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddTransient<ISecurityTokenProvider, SecurityTokenProvider>();

        var jwtConfig = builder.Configuration.GetRequiredSection("Jwt");
        builder.Services.AddOptionsWithValidateOnStart<SecurityTokenOptions>()
            .Bind(jwtConfig)
            .ValidateDataAnnotations();

        var securityTokenOptions = jwtConfig.Get<SecurityTokenOptions>()!;
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = !string.IsNullOrEmpty(securityTokenOptions.Issuer),
                    ValidIssuer = securityTokenOptions.Issuer,
                    ValidateAudience = !string.IsNullOrEmpty(securityTokenOptions.Audience),
                    ValidAudience = securityTokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(securityTokenOptions.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var sid = context.Principal!.Claims.First(c => c.Type is JwtRegisteredClaimNames.Sid);
                        var sessionManager = context.HttpContext.RequestServices.GetRequiredService<ISessionManager>();

                        if (!await sessionManager.ValidateSessionAsync(sid.Value))
                        {
                            context.Fail("Session is no longer valid. Please login again.");
                        }
                    }
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}