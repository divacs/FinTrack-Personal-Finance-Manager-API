using FinTrack.Domain.Entities;

namespace FinTrack.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<Report> GenerateMonthlyReportAsync(string userId, int year, int month);
        Task<Report> GenerateYearlyReportAsync(string userId, int year);
        Task<IEnumerable<Report>> GetMonthlyReportsAsync(string userId);
        Task<IEnumerable<Report>> GetYearlyReportsAsync(string userId);
    }
}
