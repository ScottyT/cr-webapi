using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using cr_app_webapi.Dto;
using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace cr_app_webapi.Services
{
    public class ReportsService
    {
        private readonly IMongoDatabase _database;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMongoCollection<BsonDocument> _bsonCol;
        private readonly IMongoCollection<Psychrometric> _psy;

        public ReportsService(ICodeRedDatabaseSettings settings, IMongoRepo<InventoryModel,InventoryModel> image)
        {
            _database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _contextAccessor = new HttpContextAccessor();
            _bsonCol = _database.GetCollection<BsonDocument>("reports");
            _psy = _database.GetCollection<Psychrometric>("reports");
        }

        // Also used to create chart
        public async Task UpdatePsychrometricChart(Psychrometric report, string action)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            //BsonDocument doc = BsonDocument.Parse(reportJson);
            //Psychrometric? rep = JsonSerializer.Deserialize<Psychrometric>(reportJson);
            //var jobProgress = doc.Elements.Where(el => el.Name == "jobProgress").FirstOrDefault();
            //var JobProgress = report.jobProgress.FirstOrDefault();
            //if (rep is null) return;
            var filters = (Builders<Psychrometric>.Filter.Eq("JobId", report.JobId) & Builders<Psychrometric>.Filter.Eq("ReportType", report.ReportType)
                & Builders<Psychrometric>.Filter.Eq("formType", report.formType));
            /* var filterDef = new List<FilterDefinition<Psychrometric>>();
            filterDef.Add(filters); */
            var existingReport = _psy.Find(filters).FirstOrDefault();
            /* UpdateDefinition<Psychrometric> updateBuilder = null;
            var update = updateBuilder; */
            var options = new FindOneAndUpdateOptions<Psychrometric> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            switch (action)
            {
                /* case "new":
                    updateBuilder = Builders<Psychrometric>.Update.Push(jobProgress.Name, jobProgress.Value);
                    var update = updateBuilder;
                    if (existingReport is null) update = updateBuilder.Set("updatedAt", time).Set("createdAt", time);
                    else update = updateBuilder.Set("updatedAt", time);
                    var options = new FindOneAndUpdateOptions<Psychrometric> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
                    await _psy.FindOneAndUpdateAsync(filters, update, options);
                    break; */
                case "update":
                    /* var jobProgressFilter = Builders<Psychrometric>.Filter.ElemMatch(
                        x => x.jobProgress, x => x.date == report.date); */
                    var jobProgressUpdate = Builders<Psychrometric>.Update.Set("jobProgress.$[j]", report);
                        //.Set("updatedAt", time.ToString());
                    /* filterDef.Add(Builders<Psychrometric>.Filter.ElemMatch(
                        x => x.jobProgress, x => x.date == report.date && x.readingsType == JobProgress!.readingsType)
                    ); */
                    /* var i = existingReport.jobProgress.FindIndex(x => x.date == report.date && x.readingsType == JobProgress!.readingsType);
                    var elm = existingReport.jobProgress; */
                    
                    
                    /* var arrayFilter = new[]
                    {
                        new BsonDocumentArrayFilterDefinition<BsonDocument>(
                            new BsonDocument("j.readingsType", new BsonDocument("$eq", JobProgress!.readingsType))
                        ),
                        new BsonDocumentArrayFilterDefinition<BsonDocument>(
                            new BsonDocument("j.date", new BsonDocument("$eq", JobProgress!.date))
                        )
                    }; */
                    //var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };
                    options = new FindOneAndUpdateOptions<Psychrometric> { ReturnDocument = ReturnDocument.After };
                    //filters = filters & jobProgressFilter;
                    await _psy.UpdateOneAsync(filters, jobProgressUpdate);
                    break;
                default:
                    await HandleError(_contextAccessor, "Please provide an action type");
                    break;
            }
        }

        private async Task HandleError(IHttpContextAccessor context, string message)
        {
            var httpContext = context.HttpContext;
            if (httpContext is null) return;
            await httpContext.Response.WriteAsJsonAsync("Error: " + message);
        }
    }
    public static class ValidExtensions
    {
        public static bool IsValid<T>([NotNullWhen(true)]T? doc) =>
            doc is not null;
    }
}