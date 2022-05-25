using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Dto;

[BsonIgnoreExtraElements]
public class GeneralHistoryDTO : Document
{
    /* BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;} */
    public string JobId {get; set;} = null!;
    public List<PaymentArray> payments {get; set;} = new List<PaymentArray>();
    public string? startOfJob {get; set;}
    public string? endOfJob {get; set;}
    public string? emailSentDate {get; set;}
}
public class GeneralHistoryData
{
    public GeneralHistoryDTO Data {get; set;} = new GeneralHistoryDTO();
}