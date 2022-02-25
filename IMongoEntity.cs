using MongoDB.Bson;

namespace cr_app_webapi;
public interface IMongoEntity<TId>
{
    TId Id { get; set; }
}
public interface IMongoCommon : IMongoEntity<ObjectId>
{
    string Name { get; set; }
    bool IsActive { get; set; }
    string Description { get; set; }
    DateTime Created { get; set; }
    DateTime Modified { get; set; }
}