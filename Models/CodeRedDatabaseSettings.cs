using cr_app_webapi.Services;

namespace cr_app_webapi.Models
{

    public class CodeRedDatabaseSettings : ICodeRedDatabaseSettings
    {
        public string? CodeRedCollectionName {get; init;}
        public string? ConnectionString { get; init;}
        public string? DatabaseName { get; init; }
    }
    public class AuthSettings
    {
        public string? DevCredentialPath {get; set;}
        public string? ProdCredentialPath {get; set;}
        public string? ProjectId {get; set;}
    }
    public class Auth0Settings
    {
        public string? ClientId {get; set;}
        public string? ClientSecret {get; set;}
        public string? Audience {get; set;}
        public string? ApiUrl {get; set;}
    }
    public interface ICodeRedDatabaseSettings
    {
        string? CodeRedCollectionName {get; init;}
        string? ConnectionString { get; init; }
        string? DatabaseName { get; init; }
    }
}