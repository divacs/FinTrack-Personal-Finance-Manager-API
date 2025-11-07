using FinTrack.Domain.Entities;

namespace FinTrack.Application.Interfaces
{
    public interface IReportJobLogRepository
    {
        Task AddAsync(ReportJobLogs log);
        Task<IEnumerable<ReportJobLogs>> GetLogsByUserAsync(string userId);
    }
}
