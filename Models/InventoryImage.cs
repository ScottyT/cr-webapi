using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    [BsonCollection("inventory.images")]
    public class InventoryImage : Document
    {
        public InventoryImage()
        {
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
        }
        public string? JobId {get; set;}
        public FileModel? img {get; set;}
        public string? ItemNumber {get; set;}
    }

    public class FileModel
    {
        public byte[]? data {get; set;}
        public string? contentType {get; set;}
    }
}