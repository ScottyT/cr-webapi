using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
public class TeamMember
{
    public string? name {get; set;}
    public string? email {get; set;}
    [BsonElement("id")]
    public string? employee_id {get; set;}
    public string? role {get; set;}
}