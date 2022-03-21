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
        public string JobId {get; set;} = default!;
        //public ImageModel? img {get; set;} //= new List<ImageModel>();
        public FileModel? img {get; set;}
        [BsonIgnore]
        public string? ItemNumber {get; set;}
    }

    public class FileModel
    {
        public byte[]? data {get; set;} = default!;
        public string? contentType {get; set;} = default!;
        public long size {get; set;} = default!;
        public string? filename {get; set;}
    }
}