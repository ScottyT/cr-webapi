using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    public class Dispatch
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {get; set;}
        //[BsonRepresentation(BsonType.Timestamp)]
        //public DateTime Timestamp {get; set;}
        public string JobId {get; set;} = null!;
        public string? ReportType {get; set;}
        public string? formType {get; set;}
        public string? appointmentDate {get; set;}
        public string? appointmentTime {get; set;}
        public Object? callerName {get; set;}
        public string? dateFormatted {get; set;}
        public string? emailAddress {get; set;}
        public Object? location {get; set;}
        public string? textTimeUpdate {get; set;}
        public string? phoneNumber {get; set;}
        public string? summary {get; set;}
        public Object? teamMember {get; set;}
        public string? signDate {get; set;}
        public string? signTime {get; set;}
        public DateTime createdAt {get; set;}
        public DateTime updatedAt {get; set;}

        //public Dispatch() { }
    }
}