using System.Linq.Expressions;
using System.Text.Json;
using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
namespace cr_app_webapi.Services
{
    public class MongoRepo<TDocument, TForeign> : IMongoRepo<TDocument, TForeign>
        where TDocument : IDocument, new()
    {
        private readonly IMongoCollection<TDocument> _collection;
        private readonly IMongoCollection<TForeign> _foreignCollection;

        public MongoRepo(ICodeRedDatabaseSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
            _foreignCollection = database.GetCollection<TForeign>(GetCollectionName(typeof(TForeign)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute) documentType.GetCustomAttributes(
                typeof(BsonCollectionAttribute), 
                true
            ).FirstOrDefault()).CollectionName;
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public virtual IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToEnumerable();
        }

        public IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<TDocument, bool>> filterExpression, 
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }
        
        public IEnumerable<TProjected> FindAndJoin<TProjected>(Expression<Func<TDocument, bool>> matchExpression, 
            Expression<Func<TDocument, object>> localField, Expression<Func<TForeign, object>> foreignField,
            Expression<Func<TProjected, object>> joined)
        {
            return _collection.Aggregate().Match(matchExpression).Lookup(_foreignCollection, localField, foreignField, joined).ToEnumerable();
        }

        public virtual Task<TDocument> GetOneAsync(string id)
        {
            return Task.Run(() => 
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }

        public virtual Task SaveOneAsync(string json)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            TDocument? doc = JsonSerializer.Deserialize<TDocument>(json);
            doc.updatedAt = time;
            doc.createdAt = time;
            return Task.Run(() =>  _collection.InsertOneAsync(doc));
        }

        /* public virtual Task UpdateOneAsync(string id)
        {
            var filters = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            var update = Builders<TDocument>.Update.
            return Task.Run(() => _collection.FindOneAndUpdateAsync(filters, updateExpression));
        } */

        public async Task FindOneAndUpdate<TProjected>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, 
            FindOneAndUpdateOptions<TDocument, TProjected> updateOptions)
        {
            await _collection.FindOneAndUpdateAsync(filter, update, updateOptions);
        }
        
        public virtual Task InsertOneAsync(TDocument document)
        {
            return Task.Run(() => _collection.InsertOneAsync(document));
        }
    }
}