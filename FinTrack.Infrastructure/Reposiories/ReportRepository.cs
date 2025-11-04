using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Generate monthly report for a user
        public async Task<Report> GenerateMonthlyReportAsync(string userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddTicks(-1);

            var transactions = await _context.Transactions
                .Where(t => t.BankAccount.UserId == userId && t.Date >= start && t.Date <= end)
                .ToListAsync();

            decimal totalIncome = transactions
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);

            decimal totalExpenses = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            var report = new Report
            {
                UserId = userId,
                GeneratedAt = DateTime.UtcNow,
                Month = $"{year:0000}-{month:00}",
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses
            };

            // Optionally save report in DB
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        // Generate yearly report for a user
        public async Task<Report> GenerateYearlyReportAsync(string userId, int year)
        {
            var start = new DateTime(year, 1, 1);
            var end = new DateTime(year, 12, 31, 23, 59, 59);

            var transactions = await _context.Transactions
                .Where(t => t.BankAccount.UserId == userId && t.Date >= start && t.Date <= end)
                .ToListAsync();

            decimal totalIncome = transactions
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);

            decimal totalExpenses = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            var report = new Report
            {
                UserId = userId,
                GeneratedAt = DateTime.UtcNow,
                Month = "All", // or null
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }
        // Retrieve stored reports for a user
        public async Task<IEnumerable<Report>> GetMonthlyReportsAsync(string userId)
        {
            return await _context.Reports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }
        // Retrieve stored reports for a user
        public async Task<IEnumerable<Report>> GetYearlyReportsAsync(string userId)
        {
            return await _context.Reports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }
    }
}
