using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
[BsonCollection("employees")]
public class Employee : Document
{
    public Employee()
    {
        createdAt = DateTime.Now;
        updatedAt = DateTime.Now;
    }
    public string? fname { get; set; }
    public string? lname { get; set; }
    public string? email { get; set; }
    public string? full_name { get; set; }
    [BsonElement("id")]
    public string? team_id { get; set; }
    public string? username { get; set; }
    public string? role { get; set; }
    // Might change certifications to be defined as Certification type
    //public List<string>? certifications_id { get; init; }
    public string? picture { get; set; }
    public string? auth_id {get; set;}

    // public List<Object> UserReports {get; set;} = new List<Object>();
}

public record struct FullName(string FirstName, string LastName)
{
    public string Name => FirstName + " " + LastName;
}
[BsonIgnoreExtraElements]
public class AuthUser
{
    //public Employee employee {get; set;} = new Employee();
    public string? connection { get; set; }
    public string? email { get; set; }
    public string? password { get; set; }
    public string? username { get; set; }
    public string? name { get; set; }
    public UserMetadata? user_metadata { get; set; }
}
public class UserMetadata
{
    public List<string> certifications { get; set; } = new List<string>();
    public string? role { get; set; }
    public string? id { get; set; }
    public string? name {get; set;}
}
public class UserRoleView
{
    public string? user_id { get; set; }
    public UserMetadata? user_metadata { get; set; }
}
public class UserObj
{
    public string fname { get; set; } = default!;
    public string lname { get; set;} = default!;
    public string? email { get; set;}
    [BsonElement("id")]
    public string? team_id { get; set;}
    public string? role { get; set;}
    public string? picture { get; init; }
    public string? username { get; set;}
    public string? password { get; set;}
}
public class Roles
{
    public string[] roles {get; set;} = new string[]{};
}