using System.Text.Json.Serialization;
using cr_app_webapi.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    public class Dispatch : Report
    {
        public string? appointmentDate {get; set;}
        public string? appointmentTime {get; set;}
        public string? timeFormatted {get; set;}
        public Dictionary<string, object> callerName {get; set;} = new Dictionary<string, object>();
        public Dictionary<string, object>? ArrivalContactName {get; set;}
        public string? dateFormatted {get; set;}
        public string? emailAddress {get; set;}
        public Dictionary<string, object>? location {get; set;}
        public string? textTimeUpdate {get; set;}
        public List<string> propertyChkList {get; set;} = new List<string>();
        public List<Object> intrusion {get; set;} = new List<Object>();
        public string? timeIntrusionBegan {get; set;}
        public string? timeIntrusionEnded {get; set;}
        public string? callTimeUpdate {get; set;}
        public string? phoneNumber {get; set;}
        public string? summary {get; set;}
        public string? signDate {get; set;}
        public string? signTime {get; set;}
        [BsonElement("id")]
        public string? team_id {get; set;}
    }
}