using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
[BsonCollection("sketches")]
public class Sketch : Document
{
    public Sketch()
    {
        createdAt = DateTime.UtcNow;
        updatedAt = DateTime.UtcNow;
    }
    public string JobId { get; set; } = null!;
    public string ReportType { get; set; } = default!;
    public string? formType { get; set; }
    public string? sketch { get; set; }
    [BsonElement("title")]
    public string? Title { get; set; }
    public string? notes { get; set; }
}