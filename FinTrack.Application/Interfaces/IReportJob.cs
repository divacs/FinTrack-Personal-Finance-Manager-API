using System.Threading.Tasks;

namespace FinTrack.Application.Interfaces
{
    // Handles automated generation and delivery of monthly and yearly financial reports.

    public interface IReportJob
    {
        // Sends monthly financial summaries to all active users.
        Task SendMonthlyReportsAsync();

        // Sends yearly financial summaries to all active users.
        Task SendYearlyReportsAsync();

        // Sends a monthly report to a specific user by ID.
        Task SendMonthlyReportAsync(string userId);

        // Sends a yearly report to a specific user by ID.
        Task SendYearlyReportAsync(string userId);
    }
}
