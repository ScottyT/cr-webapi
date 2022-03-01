using cr_app_webapi.Models;

namespace cr_app_webapi.Services
{
    public interface IReportService
    {
        Task<Object?> GetReport(string reportid);
        List<Object> GetContract(string reportType, string id);
        Task CreateReport(string reportType, string report);
        Task UpdatePsychrometricChart(string reportJson, Psychrometric report, string action);
        Task UpdateReport(string reportType, string jobId);
    }
}