namespace cr_app_webapi.Models
{

    public class CodeRedDatabaseSettings : ICodeRedDatabaseSettings
    {
        public string CodeRedCollectionName {get; set;} = null!;
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }

    public class ICodeRedDatabaseSettings
    {
        string CodeRedCollectionName {get; set;} = null!;
        string ConnectionString { get; set; } = null!;
        string DatabaseName { get; set; } = null!;
    }
}