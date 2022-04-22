using Microsoft.AspNetCore.Authorization;

public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        
        var user = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer);
        Console.WriteLine("authorization context: ", user);
        if (user is null)
            return Task.CompletedTask;
        var claim = context.User.Claims;
        // If user does not have the scope claim, get out of here
        if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
            return Task.CompletedTask;

        // Split the scopes string into an array
        var scopes = user.Value.Split(' ');
        var permissions = context.User.FindAll(c => c.Type == "permissions" && c.Issuer == requirement.Issuer).Select(p => p.Value);
        var allScopes = scopes.Concat(permissions);

        // Succeed if the scope array contains the required scope
        if (allScopes.Any(s => s == requirement.Scope))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
