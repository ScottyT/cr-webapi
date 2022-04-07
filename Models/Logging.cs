using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    [BsonCollection("reports")]
    public class Logging : Report
    {
        public string? startDate {get; set;}
        public string? endDate {get; set;}
        public List<object> readingsLog {get; set;} = new List<object>();
        public List<object> lossClassification {get; set;} = new List<object>();
        public List<object> quantityData {get; set;} = new List<object>();
        public List<object> checkData {get; set;} = new List<object>();
        public List<object> categoryData {get; set;} = new List<object>();
        public string? notes {get; set;}
        public List<object> serviceArr {get ;set;} = new List<object>();
        public Location location {get; set;} = new Location();
    }
}