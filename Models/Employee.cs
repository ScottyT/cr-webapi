using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
[BsonKnownTypes(typeof(Employee))]
public class Employee : IEmployee
{
    public Employee()
    {
        IsActive = true;
        Created = DateTime.UtcNow;
        Modified = DateTime.UtcNow;
    }
    [BsonId]
    public ObjectId Id { get; set; }
    public string fname { get; init; } = default!;
    public string lname { get; init; } = default!;
    public FullName fullName {get; init;} = default!;
    public string email { get; init; } = default!;
    public string id { get; init; } = default!;
    public string role { get; init; } = default!;
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    // Might change certifications to be defined as Certification type
    public List<string> certifications_id { get; set; } = new List<string>();
    public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsActive { get; set; }
    public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime Created { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime Modified { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    // public List<Object> UserReports {get; set;} = new List<Object>();
}

public interface IEmployee : IMongoCommon
{
    string fname { get; init; }
    string lname { get; init; }
    FullName fullName {get; init;}
    string email { get; init; }
    string id { get; init; }
    string role { get; init; }
}

public record FullName(string FirstName, string LastName);

public class Certification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    public string? idNumber { get; set; }
    public string? description { get; set; }
    public string? expiration { get; set; }
    public Object? badge { get; set; }
}