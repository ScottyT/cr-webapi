using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using cr_app_webapi.Dto;
using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace cr_app_webapi.Services
{
    public class MongoRepo<TDocument, TForeign> : IMongoRepo<TDocument, TForeign>
        where TDocument : IDocument, new()
    {
        private readonly IMongoCollection<TDocument> _collection;
        private readonly IMongoCollection<TForeign> _foreignCollection;
        private readonly IMongoCollection<BsonDocument> _bsonCol;

        public MongoRepo(ICodeRedDatabaseSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
            _foreignCollection = database.GetCollection<TForeign>(GetCollectionName(typeof(TForeign)));
            _bsonCol = database.GetCollection<BsonDocument>(GetCollectionName(typeof(TDocument)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
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
            return _collection.Find(filterExpression).Project(projectionExpression).As<TProjected>().ToEnumerable();
        }

        public IEnumerable<TDocument> FindAndJoin(Expression<Func<TDocument, bool>> matchExpression,
            Expression<Func<TDocument, object>> localField, Expression<Func<TForeign, object>> foreignField,
            Expression<Func<TDocument, object>> joined)
        {
            return _collection.Aggregate().Match(matchExpression).Lookup(_foreignCollection, localField, foreignField, joined).ToEnumerable();
        }

        public IEnumerable<TProjected> FindAndJoin<TProjected>(Expression<Func<TDocument, bool>> matchExpression,
            Expression<Func<TDocument, object>> localField, Expression<Func<TForeign, object>> foreignField,
            Expression<Func<TDocument, object>> joined, Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            return _collection.Aggregate().Match(matchExpression).Lookup(_foreignCollection, localField, foreignField, joined)
                .Project(projectionExpression).ToList().AsQueryable();
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

        public async Task BsonFindOneAndUpdate(string jobid, string reportType, string json, bool upsert)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            BsonDocument doc = BsonDocument.Parse(json);
            var filter = Builders<BsonDocument>.Filter.Eq("JobId", jobid) & Builders<BsonDocument>.Filter.Eq("ReportType", reportType);
            var updateOptions = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = upsert, ReturnDocument = ReturnDocument.After };
            var updateDef = new List<UpdateDefinition<BsonDocument>>();
            var existingReport = await _bsonCol.Find(filter).FirstOrDefaultAsync();
            if (existingReport is null)
            {
                updateDef.Add(Builders<BsonDocument>.Update.Set("createdAt", time));
            }
            foreach (BsonElement field in doc.Elements)
            {
                updateDef.Add(Builders<BsonDocument>.Update.Set(field.Name, field.Value));
            }
            updateDef.Add(Builders<BsonDocument>.Update.Set("updatedAt", time));
            var update = Builders<BsonDocument>.Update.Combine(updateDef);
            await _bsonCol.FindOneAndUpdateAsync(filter, update, updateOptions);
        }

        public virtual async Task BsonSaveOneAsync(string json)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var doc = BsonDocument.Parse(json);
            doc.Add("createdAt", time);
            doc.Add("updatedAt", time);
            await _bsonCol.InsertOneAsync(doc);
        }

        public virtual async Task InsertOneAsync(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public virtual async Task GenericFindOneUpdate<TProjected>(
            Expression<Func<TDocument, bool>> filter, TDocument document,
            BsonDocumentArrayFilterDefinition<BsonDocument>[]? arrFilter = null,
            UpdateDefinition<TDocument>? setArrUpdate = null,
            bool upsert = false, string action = "")
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var existingReport = _collection.Find(filter).FirstOrDefault();
            var updateDefList = new List<UpdateDefinition<TDocument>>();
            var projectionBuilder = new List<ProjectionDefinition<TDocument, TProjected>>();
            if (existingReport is null)
            {
                updateDefList.Add(Builders<TDocument>.Update.Set("createdAt", time));
            }
            else
            {
                document.Id = existingReport.Id;
            }

            if (action == "")
            {
                foreach (PropertyInfo field in document.GetType().GetProperties())
                {
                    if (field.GetValue(document, null) is not null)
                    {
                        updateDefList.Add(Builders<TDocument>.Update.Set(field.Name, field.GetValue(document, null)));
                    }
                }
            }

            updateDefList.Add(Builders<TDocument>.Update.Set("updatedAt", time));
            var updateOptions = new FindOneAndUpdateOptions<TDocument, TProjected>
            {
                IsUpsert = upsert,
                ReturnDocument = ReturnDocument.After
            };
            var update = Builders<TDocument>.Update.Combine(updateDefList);
            switch (action)
            {
                case "new":
                    await _collection.InsertOneAsync(document);
                    break;
                case "update":
                    updateDefList.Add(setArrUpdate!);
                    update = Builders<TDocument>.Update.Combine(updateDefList);
                    await _collection.UpdateOneAsync(filter, update);
                    break;
                default:
                    update = Builders<TDocument>.Update.Combine(updateDefList);
                    await _collection.FindOneAndUpdateAsync(filter, update, updateOptions);
                    break;
            }
        }

        public void DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByIdAsync(Expression<Func<TDocument, bool>> filter)
        {
            await _collection.DeleteOneAsync(filter);
        }

        public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }

        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }
    }
}