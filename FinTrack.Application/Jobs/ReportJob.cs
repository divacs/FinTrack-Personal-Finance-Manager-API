using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using System;

namespace FinTrack.Application.Jobs
{
    public class ReportJob
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IReportJobLogRepository _logRepository;

        public ReportJob(
            IReportRepository reportRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IReportJobLogRepository logRepository)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _logRepository = logRepository;
        }

        /// <summary>
        /// Sends monthly report emails to all users.
        /// </summary>
        public async Task SendMonthlyReportsAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var now = DateTime.UtcNow;

            foreach (var user in users)
            {
                try
                {
                    var report = await _reportRepository.GenerateMonthlyReportAsync(user.Id, now.Year, now.Month);

                    var emailBody = $"Your {report.Month} report is ready.\n" +
                                    $"Total income: {report.TotalIncome:C}\n" +
                                    $"Total expenses: {report.TotalExpenses:C}\n" +
                                    $"Net balance: {report.NetBalance:C}";

                    await _emailService.SendEmailAsync(user.Email!, "Monthly Financial Report", emailBody);

                    await _logRepository.AddAsync(new ReportJobLogs
                    {
                        UserId = user.Id,
                        ReportType = "Monthly",
                        Period = $"{now:yyyy-MM}",
                        IsSuccess = true
                    });
                }
                catch (Exception ex)
                {
                    await _logRepository.AddAsync(new ReportJobLogs
                    {
                        UserId = user.Id,
                        ReportType = "Monthly",
                        Period = $"{now:yyyy-MM}",
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    });
                }
            }
        }

        /// <summary>
        /// Sends yearly report emails to all users.
        /// </summary>
        public async Task SendYearlyReportsAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var year = DateTime.UtcNow.Year;

            foreach (var user in users)
            {
                try
                {
                    var report = await _reportRepository.GenerateYearlyReportAsync(user.Id, year);

                    var emailBody = $"Your {year} financial report is ready.\n" +
                                    $"Total income: {report.TotalIncome:C}\n" +
                                    $"Total expenses: {report.TotalExpenses:C}\n" +
                                    $"Net balance: {report.NetBalance:C}";

                    await _emailService.SendEmailAsync(user.Email!, "Yearly Financial Report", emailBody);

                    await _logRepository.AddAsync(new ReportJobLogs
                    {
                        UserId = user.Id,
                        ReportType = "Yearly",
                        Period = year.ToString(),
                        IsSuccess = true
                    });
                }
                catch (Exception ex)
                {
                    await _logRepository.AddAsync(new ReportJobLogs
                    {
                        UserId = user.Id,
                        ReportType = "Yearly",
                        Period = year.ToString(),
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    });
                }
            }
        }
    }
}
