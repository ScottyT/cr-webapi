using System.Linq.Expressions;
using MongoDB.Driver;

namespace cr_app_webapi.Services;
public interface IMongoRepo<TDocument, TForeign> where TDocument : IDocument, new()
{
    IQueryable<TDocument> AsQueryable();
    IEnumerable<TDocument> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression);
    IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression);

    IEnumerable<TProjected> FindAndJoin<TProjected>(
        Expression<Func<TDocument, bool>> matchExpression, 
        Expression<Func<TDocument, object>> localField, Expression<Func<TForeign, object>> foreignField,
        Expression<Func<TProjected, object>> joined
    );
    Task<TDocument> GetOneAsync(string id);
    //Task<TDocument> GetManyAsync(IEnumerable<string> ids);
    Task SaveOneAsync(string json);
    //Task UpdateOneAsync(string id);
    Task InsertOneAsync(TDocument document); // This method is just like SaveOneAsync except in takes a generic as a parameter
    Task FindOneAndUpdate<TProjected>(
        FilterDefinition<TDocument> filter,
        UpdateDefinition<TDocument> update,
        FindOneAndUpdateOptions<TDocument, TProjected> updateOptions);
}