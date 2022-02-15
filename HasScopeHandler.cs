using cr_app_webapi.Models;
using JwtAuthentication.AsymmetricEncryption.Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    private readonly SigningAudienceCertificate signingAudienceCertificate;

    public HasScopeHandler(IConfiguration config, IOptions<Auth0Settings> settings)
    {
        signingAudienceCertificate = new SigningAudienceCertificate();
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        var user = context.User;
        var claim = context.User.Claims;
        var issuer = context.User.Claims.Select(c => c.Issuer);
        // If user does not have the scope claim, get out of here
        if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
            return Task.CompletedTask;

        // Split the scopes string into an array
        var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer).Value.Split(' ');
        var permissions = context.User.FindAll(c => c.Type == "permissions" && c.Issuer == requirement.Issuer).Select(p => p.Value);
        var allScopes = scopes.Concat(permissions);

        // Succeed if the scope array contains the required scope
        if (allScopes.Any(s => s == requirement.Scope))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
