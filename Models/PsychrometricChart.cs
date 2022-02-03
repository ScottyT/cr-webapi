using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
public class Psychrometric
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id {get; set;}
    public string? JobId {get; set;}
    public string? ReportType {get; set;}
    public string? formType {get; set;}
    public Object? teamMember {get; set;}
    public JobProgress? jobProgress {get; set;}
    public DateTime createdAt {get; set;}
    public DateTime updatedAt {get; set;}
}

public class JobProgress
{
    public Object? info {get; set;}
    public string? date {get; set;}
    public string? color {get; set;}
    public string? readingsType {get; set;}
}