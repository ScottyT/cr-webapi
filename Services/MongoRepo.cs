using System.Linq.Expressions;
using System.Text.Json;
using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
namespace cr_app_webapi.Services
{
    public class MongoRepo<TDocument> : IMongoRepo<TDocument>
        where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRepo(ICodeRedDatabaseSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
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

        public virtual Task UpdateOneAsync(Expression<Func<TDocument, bool>> filterExression,
            UpdateDefinition<TDocument> updateExpression)
        {
            return Task.Run(() => _collection.UpdateOneAsync(filterExression, updateExpression));
        }
    }
}