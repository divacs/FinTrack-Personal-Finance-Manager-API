using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositories
{
    public class ReportJobLogRepository : IReportJobLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportJobLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReportJobLogs log)
        {
            _context.ReportJobLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReportJobLogs>> GetLogsByUserAsync(string userId)
        {
            return await _context.ReportJobLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
    }
}
