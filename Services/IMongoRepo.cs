using System.Linq.Expressions;
using MongoDB.Driver;

namespace cr_app_webapi.Services;
public interface IMongoRepo<TDocument> where TDocument : IDocument
{
    IQueryable<TDocument> AsQueryable();
    IEnumerable<TDocument> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression);
    IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression);
    Task<TDocument> GetOneAsync(string id);
    //Task<TDocument> GetManyAsync(IEnumerable<string> ids);
    Task SaveOneAsync(string json);
    //Task UpdateOneAsync(string id);
    Task FindOneAndUpdate(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update);
}