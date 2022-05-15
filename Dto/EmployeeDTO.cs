using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Dto;

[BsonIgnoreExtraElements]
public class EmployeeDTO
{
    public string? email {get; set;}
    public string? fullName {get; set;}
    [BsonElement("id")]
    public string? team_id {get; set;}
    public string? role {get; set;}
    public string? picture {get; set;}
    public string? auth_id {get; set;}
}