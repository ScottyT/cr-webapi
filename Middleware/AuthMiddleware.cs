using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Options;
using RestSharp;

namespace cr_app_webapi.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    private readonly string? _clientId;
    private readonly string? _clientSecret;

    public AuthMiddleware(RequestDelegate next, IConfiguration config, IOptions<Auth0Settings> settings)
    {
        _next = next;
        _clientId = settings.Value.ClientId;
        _clientSecret = settings.Value.ClientSecret;
        _config = config;
    }
    
    
    /* public async Task InvokeAsync(HttpContext context)
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
    } */

    public async Task InvokeAsync(HttpContext context)
    {
        /* var client = new RestClient("https://code-red-app.us.auth0.com/oauth/token");
        */
        
        
        
        try
        {
            
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAuthError(context, ex);
        }
        //var decodedToken = await _auth0services.GetToken();
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