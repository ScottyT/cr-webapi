using cr_app_webapi.Models;
using Microsoft.Extensions.Options;
using RestSharp;

namespace cr_app_webapi.Services
{
    public class AuthServices : IDisposable
    {
        private readonly IConfiguration _config;
        private readonly RestClient _client;
        private readonly string? _clientId;
        private readonly string? _clientSecret;

        public AuthServices(IOptions<Auth0Settings> settings, IConfiguration config)
        {
            _config = config;
            _clientId = settings.Value.ClientId;
            _clientSecret = settings.Value.ClientSecret;
            var options = new RestClientOptions(_config["Auth0:Authority"]);
            _client = new RestClient(options);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<string> GetToken()
        {
            var request = new RestRequest("oauth/token");
            var body =  new {
                client_id = _clientId,
                client_secret = _clientSecret,
                audience = "https://" + _config["Auth0:Domain"] + "/api/v2/",
                grant_type = "client_credentials"
            };
            request.AddObject(body);
            var response = await _client.PostAsync<TokenResponse>(request);
            return response!.AccessToken;
        }
    
        public async Task<RestResponse> GetUser(string userid)
        {
            var token = await GetToken();
            using var client = new RestClient("https://" + _config["Auth0:Domain"] + "/api/v2/");
            var request = new RestRequest("users/" + userid);
            request.AddHeader("authorization", "Bearer "+ token);
            var response = await client.GetAsync(request);
    
            return response;
        }
    }
}