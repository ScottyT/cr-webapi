using System.Linq.Expressions;
using cr_app_webapi.Dto;
using MongoDB.Bson;
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

    IEnumerable<TDocument> FindAndJoin(
        Expression<Func<TDocument, bool>> matchExpression, 
        Expression<Func<TDocument, object>> localField, Expression<Func<TForeign, object>> foreignField,
        Expression<Func<TDocument, object>> joined
    );
    IEnumerable<TProjected> FindAndJoin<TProjected>(
        Expression<Func<TDocument, bool>> matchExpression, 
        Expression<Func<TDocument, object>> localField, Expression<Func<TForeign, object>> foreignField,
        Expression<Func<TDocument, object>> joined, Expression<Func<TDocument, TProjected>> projectionExpression
    );
    Task<TDocument> GetOneAsync(string id);
    //Task<TDocument> GetManyAsync(IEnumerable<string> ids);
    Task BsonSaveOneAsync(string json);
    Task BsonFindOneAndUpdate(string jobid, string reportType, string json, bool upsert);
    Task InsertOneAsync(TDocument document); // This method is just like SaveOneAsync except in takes a generic as a parameter. Can't do projections.

    // Keep the arrFilter, setArrUpdate, and action params the default unless used for Psychrometric type
    Task GenericFindOneUpdate<TProjected>(
        Expression<Func<TDocument, bool>> filter,
        TDocument document, BsonDocumentArrayFilterDefinition<BsonDocument>[]? arrFilter = null,
        UpdateDefinition<TDocument>? setArrUpdate = null,
        bool upsert = false, string action = ""
    );
    void DeleteById(string id);

    Task DeleteByIdAsync(Expression<Func<TDocument, bool>> filter);

    void DeleteMany(Expression<Func<TDocument, bool>> filterExpression);

    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
}