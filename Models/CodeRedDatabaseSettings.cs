using FirebaseAdmin.Auth;
namespace cr_app_webapi.Models
{

    public class CodeRedDatabaseSettings : ICodeRedDatabaseSettings
    {
        public string? CodeRedCollectionName {get; set;}
        public string? ConnectionString { get; set;}
        public string? DatabaseName { get; set; }
    }
    public class AuthSettings
    {
        public string? CredentialPath {get; set;}
        public string? ProjectId {get; set;}
    }

    public class ICodeRedDatabaseSettings
    {
        string? CodeRedCollectionName {get; set;}
        string? ConnectionString { get; set; }
        string? DatabaseName { get; set; }
    }
}