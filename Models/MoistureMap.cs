using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    [BsonCollection("reports")]
    public class MoistureModel : Report
    {
        public string? initialEvalDate {get; set;}
        public Location location {get ;set;} = new Location();
        public List<object>? baselineReadings {get; set;}
        public List<object>? readingsRow {get; set;}
        public string? notes {get; set;}
    }
}