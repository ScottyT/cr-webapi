using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
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
        public int __v {get; set;}
        // Might change certifications to be defined as Certification type
        public List<Object> certifications {get; set;} = new List<Object>();
        public List<Object> UserReports {get; set;} = new List<Object>();
    }
}