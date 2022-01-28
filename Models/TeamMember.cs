using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
public class TeamMember
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id {get; set;}
    public string? fname {get; set;}
    public string? lname {get; set;}
    public string? email {get; set;}
    public string? id {get; set;}
    public string? role {get; set;}
    public DateTime createdAt {get; set;}
    public DateTime updatedAt {get; set;}
}