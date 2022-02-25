using MongoDB.Bson;
using MongoDB.Driver;
namespace cr_app_webapi.Services;
public class GlobalLogic<TCollection, TContext>
  where TCollection : IMongoCommon
  where TContext : IMongoCommon, new()
{
    public async Task<IEnumerable<TCollection>> GetAllAsync(IMongoCollection<TCollection> collection)
    {
        return await collection.Find(f => true).ToListAsync();
    }

    public async Task<TCollection> GetOneAsync(IMongoCollection<TCollection> collection, TContext context)
    {
        return await collection.Find(new BsonDocument("_id", context.Id)).FirstOrDefaultAsync();
    }

    public async Task<TCollection> GetOneAsync(IMongoCollection<TCollection> collection, string id)
    {
        return await GetOneAsync(collection, new TContext { Id = new ObjectId(id) });
    }

    public async Task<IEnumerable<TCollection>> GetManyAsync(IMongoCollection<TCollection> collection,
                                                             IEnumerable<TContext> contexts)
    {
        var list = new List<TCollection>();
        foreach (var context in contexts)
        {
            var doc = await GetOneAsync(collection, context);
            if (doc == null) continue;
            list.Add(doc);
        }

        return list;
    }

    public async Task<IEnumerable<TCollection>> GetManyAsync(IMongoCollection<TCollection> collection,
                                                             IEnumerable<string> ids)
    {
        var list = new List<TCollection>();
        foreach (var id in ids)
        {
            var doc = await GetOneAsync(collection, id);
            if (doc == null) continue;
            list.Add(doc);
        }

        return list;
    }
}