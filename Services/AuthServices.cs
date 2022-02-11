using System.Net.Http.Headers;
using cr_app_webapi.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http;
using RestSharp;
using RestSharp.Authenticators;
using System.Text;
using JWT.Builder;
using System.Security.Cryptography.X509Certificates;
using JWT.Algorithms;

namespace cr_app_webapi.Services
{
    public class AuthServices
    {
        private readonly IConfiguration _config;
        public record Auth0User(string id);
        record Auth0SingleObject<T>(T Data);

        public AuthServices(IConfiguration config)
        {
            _config = config;
        }
        
        public string GetIdToken()
        {
            return _config["Auth0:Audience"];
        }
    
        public async Task<Object> GetUser(string userid)
        {
            //HttpContext context = new HttpContext();
           // Auth0User user = new Auth0User(userid);
            Object user = new Object();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_config["Auth0:Audience"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idtoken);
            HttpResponseMessage response = await client.GetAsync("users/" + userid);
            Console.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {
                user = response.Content.ReadFromJsonAsync<Object>();
            }
            return user;
        }

        public async Task Get(string idToken)
        {
            /* var certPem = 
            var cert = new X509Certificate2(Convert.FromBase64String(eccPem));


            var token = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(cert))
                .MustVerifySignature()
                .Decode(idToken);
            Console.WriteLine(token); */
        }
    }
}