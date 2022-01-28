using System.Net.Http.Headers;
using cr_app_webapi.Services;

namespace cr_app_webapi.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthServices _authServices;

    public AuthMiddleware(RequestDelegate next, AuthServices authServices)
    {
        _next = next;
        _authServices = authServices;
    }
    
    
    public async Task InvokeAsync(HttpContext context)
    {
        var headerAuth = context.Request.Headers.Authorization.ToString();
        //Boolean hasBearer = headerAuth.Split(' ')[0].Equals("Bearer");
        try
        {
            var decodedToken = await _authServices.Verify(headerAuth.Split(' ')[1]);
            context.Items["authId"] = decodedToken;
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAuthError(context, ex);
        }
    }

    private async Task HandleAuthError(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync("Authentication Error: " + ex.Message);
    }
}
public static class AuthMiddlewareExtensions
{
    public static IApplicationBuilder UseAuth(this IApplicationBuilder builder)
    {
        
        return builder.UseMiddleware<AuthMiddleware>();
    }
}