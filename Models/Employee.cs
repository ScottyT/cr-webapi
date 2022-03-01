using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
[BsonCollection("employees")]
public class Employee : Document
{
    public string fname { get; init; } = default!;
    public string lname { get; init; } = default!;
    public FullName? fullName {get; set;}
    public string email { get; init; } = default!;
    [BsonElement("id")]
    public string team_id { get; init; } = default!;
    public string role { get; init; } = default!;
    // Might change certifications to be defined as Certification type
    public List<string>? certifications_id { get; init; }
    public string? picture {get; set;}

    // public List<Object> UserReports {get; set;} = new List<Object>();
}

public readonly record struct FullName(string FirstName, string LastName)
{
    public string Name => FirstName + " " + LastName;
}
public class AuthUser
{
    //public Employee employee {get; set;} = new Employee();
    public string? connection {get; set;}
    public string? email {get; set;}
    public string? password {get; set;}
    public string? username {get; set;}
    public object? user_metadata {get; set;} = new object();
}
public class UserObj
{
    public Employee employee {get; set;} = new Employee();
    public AuthUser authUser {get; set;} = new AuthUser();
}