using System.Text.Json.Serialization;

namespace cr_app_webapi.Models
{
    [BsonCollection("certifications")]
    public class Certification : Document
    {
        public string? idNumber { get; set; }
        public string? description { get; set; }
        public string? expiration { get; set; }
        public Badge? badge { get; set; }
    }
    public record Badge(string imageUrl, string name);
}