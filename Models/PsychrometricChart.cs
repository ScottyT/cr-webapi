using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
public class Psychrometric : Report
{
    public JobProgress? jobProgress {get; set;}
}

public class JobProgress
{
    public Object? info {get; set;}
    public string? date {get; set;}
    public string? color {get; set;}
    public string? readingsType {get; set;}
}