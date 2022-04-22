using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using cr_app_webapi.Services;
using JWT.Builder;
using JWT.Algorithms;

namespace cr_app_webapi.Helpers;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }
    public async Task Invoke(HttpContext context, AuthServices authServices)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();  // await authServices.GetToken();
        Console.WriteLine("Token: ", token);
        if (token != null)
            attachUserToContext(context, authServices, token!);

        await _next(context);
    }

    private void attachUserToContext(HttpContext context, AuthServices authServices, string token)
    {
        try
        {
            Console.WriteLine("context:", context);
            var cert = new X509Certificate2(Convert.FromBase64String(_appSettings.CertPem));
            Console.WriteLine("Cert: ", cert);
            var claims = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(cert))
                .MustVerifySignature()
                .Decode<IDictionary<string, object>>(token);
            Console.WriteLine("claims: ", claims);
            var userId = claims["sub"].ToString();
            context.Items["User"] = authServices.GetUser(userId!);
        }
        catch
        {
            Console.WriteLine("error");
        }
    }
}