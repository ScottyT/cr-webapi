using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    [BsonCollection("reports")]
    public class Report : Document
    {
        public Report()
        {
            createdAt = DateTime.UtcNow;
            updatedAt = DateTime.UtcNow;
        }
        public string JobId {get; set;} = null!;
        public string ReportType {get; set;} = default!;
        public string? formType {get; set;}
        public TeamMember teamMember {get; set;} = new TeamMember();
        public string? date {get; set;}

        //[JsonConverter(typeof(ListObjectJsonConverter))]
        public List<object>? evaluationLogs {get; set;}
    }
}