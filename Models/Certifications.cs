using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonCollection("certifications")]
    public class Certification : Document
    {
        public Certification()
        {
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
        }
        public string? idNumber { get; set; }
        public string? description { get; set; }
        public string? expiration { get; set; }
        public Badge? badge { get; set; }
        public string teamMemberAuthId {get; set;} = default!;
    }
    public record Badge(string imageUrl, string name);
}