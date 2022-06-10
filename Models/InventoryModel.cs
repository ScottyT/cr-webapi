using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    [BsonCollection("reports")]
    public class InventoryModel : Report
    {
        public string? customer {get; set;}
        public string? claimNumber {get; set;}
        public string? insurance {get; set;}
        public string? dateCompleted {get; set;}
        public List<object> inventory {get; set;} = new List<object>();
        public Boolean techSig {get; set;}
        public string? cusSig {get ;set;}
        public double totalAmount {get; set;}
    }
}