using System.Reflection;
using System.Text.Json;
using cr_app_webapi.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Serializers;

namespace cr_app_webapi.Services
{
    public class AuthServices : IDisposable
    {
        private readonly IConfiguration _config;
        private readonly RestClient _client;
        private readonly IMongoRepo<Employee, Employee> _userRepo;
        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string? _authority;

        public AuthServices(IOptions<Auth0Settings> settings, IConfiguration config, IMongoRepo<Employee, Employee> userRepo)
        {
            _config = config;
            _clientId = settings.Value.ClientId;
            _clientSecret = settings.Value.ClientSecret;
            _authority = settings.Value.Authority;
            var options = new RestClientOptions(settings.Value.Authority + "api/v2/");
            _client = new RestClient(options);
            _userRepo = userRepo;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<TokenResponse?> GetToken()
        {
            TokenResponse? response = new TokenResponse();
            using var client = new RestClient(_authority!); //new RestClient(_config["Auth0:Authority"]);
            var request = new RestRequest("oauth/token");
            var body =  new {
                client_id = _clientId,
                client_secret = _clientSecret,
                audience = _authority + "api/v2/",
                grant_type = "client_credentials"
            };
            request.AddObject(body);
            response = await client.PostAsync<TokenResponse>(request);
            
            return response;
        }
    
        public async Task<RestResponse> GetUser(string userid)
        {
            var user = _userRepo.FilterBy(
                filter => filter.auth_id == userid,
                project => new
                {
                    email = project.email,
                    fullName = project.name,
                    team_id = project.team_id,
                    role = project.role,
                    picture = project.picture,
                    auth_id = project.auth_id
                }
            ).FirstOrDefault();
            if (user == null) return null!;
            var token = await GetToken();
            var request = new RestRequest("users/" + userid);
            request.AddHeader("authorization", "Bearer "+ token?.AccessToken);
            var response = await _client.GetAsync(request);

            return response;
        }

        public async Task<RestResponse> UpdateUser(Employee updatedUser, string userid)
        {
            var token = await GetToken();
            
            var request = new RestRequest($"users/{userid}", Method.Patch);
            var user = JsonDocument.Parse(JsonSerializer.Serialize(updatedUser));
            
            var type = updatedUser.GetType();
            
            IList<PropertyInfo> fields = new List<PropertyInfo>(type.GetProperties());
            
            request.AddHeader("Authorization", "Bearer " + token?.AccessToken);
            request.AddHeader("Accept", "application/json");
            foreach(PropertyInfo field in fields)
            {
                if (field.GetValue(updatedUser, null) is not null)
                {
                    request.AddParameter(field.Name, field.GetValue(updatedUser, null), ParameterType.RequestBody);
                }
            }
            //request.AddJsonBody(updatedUser);
            RestResponse? response = await _client.ExecuteAsync(request);
            return response!;
        }

        public async Task<RestResponse> CreateUser(AuthUser newUser)
        {
            var token = await GetToken();
            
            var request = new RestRequest("users", Method.Post);
            request.AddHeader("Authorization", "Bearer " + token?.AccessToken);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(newUser);
            RestResponse? response = await _client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var authUser = JsonSerializer.Deserialize<UserRoleView>(response?.Content!);
                await AssignRole(token?.AccessToken!, authUser?.user_id!, authUser?.user_metadata?.role!);
            }
            
            return response!;
        }

        public async Task AssignRole(string token, string userId, string role)
        {
            string[] roleid = new string[1];
            if (role == "user")
            {
                roleid[0] = "rol_HLx9h2U70mFCA5t7";
            }
            if (role == "admin")
            {
                roleid[0] = "rol_qgRRjvLYb9pRLOEL";
            }
            
            var body = new Roles {
                roles = roleid
            };

            var request = new RestRequest($"users/{userId}/roles", Method.Post);
            request.AddHeader("authorization", "Bearer " + token);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(body);
            var response = await _client.ExecuteAsync(request);
        }
    }
}