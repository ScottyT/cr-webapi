using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id {get; set;}
        public string fname {get; set;} = null!;
        public string lname {get; set;} = null!;
        public string email {get; set;} = null!;
        public string id {get; set;} = null!;
        public string role {get; set;} = null!;
        public DateTime createdAt {get; set;}
        public DateTime updatedAt {get; set;}
        // Might change certifications to be defined as Certification type
        public List<string> certifications_id {get; set;} = new List<string>();
        public List<Object> UserReports {get; set;} = new List<Object>();
    }

    public class Certification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id {get; set;}
        public string? idNumber {get; set;}
        public string? description {get; set;}
        public string? expiration {get; set;}
        public Object? badge {get; set;}
    }