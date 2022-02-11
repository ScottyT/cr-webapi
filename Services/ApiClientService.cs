
using cr_app_webapi.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace cr_app_webapi.Services
{
    /* public interface IAuth0Client
    {
        Task<Auth0User> GetUser(string user);
    }
    public record Auth0User(string id, string name);

    public class Auth0Client : IAuth0Client, IDisposable
    {
        readonly RestSharp.RestClient _client;
        public Auth0Client(string apiKey, string apiKeySecret)
        {
            var options = new RestClientOptions("https://code-red-app.us.auth0.com/api/v2/");
            _client = new RestClient(options) {
                Authenticator = new Auth0Authenticator("https://code-red-app.us.auth0.com/oauth/token", apiKey, apiKeySecret)
            };
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Auth0User> GetUser(string user)
        {
            var response = await _client.GetJsonAsync<Auth0SingleObject<Auth0User>>("users/{user}", new { user });
            return response!.Data;
        }

        record Auth0SingleObject<T>(T Data);
    } */
    public class Auth0Authenticator : AuthenticatorBase
    {
        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        public Auth0Authenticator(string baseUrl, string clientId, string clientSecret) : base("")
        {
            _baseUrl      = baseUrl;
            _clientId     = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<string> GetToken()
        {
            var options = new RestClientOptions(_baseUrl);
            using var client = new RestClient(options)
            {
                Authenticator = new HttpBasicAuthenticator(_clientId, _clientSecret),
            };
            //var client = new RestClient("https://code-red-app.us.auth0.com/oauth/token");
            var request = new RestRequest("oauth2/token", Method.Post)
                .AddHeader("content-type", "application/json")
                .AddParameter("application/json", "{\"client_id\":\"5bYqZcXOUmH3G9ZYZqYRUZUGXbPkYooz\",\"client_secret\":\"BC4pQi6wRrVggCjFJvCnYThyenj8gwkcMscN66WTEHv7axtDH6gbT4rIo_PeRmZn\",\"audience\":\"https://code-red-app.us.auth0.com/api/v2/\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            var response = await client.PostAsync<TokenResponse>(request);
            //RestResponse response = await client.ExecuteAsync(request);
            //return response.Content;
            return $"{response!.TokenType} {response!.AccessToken}";
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        {
            var token = string.IsNullOrEmpty(Token) ? await GetToken() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, token);
        }
    }
}

