using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
[BsonCollection("sketches")]
public class Sketch : Report
{
    public string? sketch {get; set;}
    [BsonElement("title")]
    public string? Title {get; set;}
    public string? notes {get; set;}
}