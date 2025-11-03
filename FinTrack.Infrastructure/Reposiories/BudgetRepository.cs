using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ApplicationDbContext _context;

        public BudgetRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // Get all budgets for a specific user
        public async Task<IEnumerable<Budget>> GetUserBudgetsAsync(string userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
        // Get budget by id for a specific user
        public async Task<Budget?> GetByIdAsync(int id, string userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }
        // Add new budget for a specific user
        public async Task<Budget> AddAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return budget;
        }
        // Update budget for a specific user
        public async Task<Budget?> UpdateAsync(Budget budget)
        {
            var existing = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == budget.Id && b.UserId == budget.UserId);
            if (existing == null) return null;

            existing.CategoryId = budget.CategoryId;
            existing.LimitAmount = budget.LimitAmount;
            existing.StartDate = budget.StartDate;
            existing.EndDate = budget.EndDate;

            await _context.SaveChangesAsync();
            return existing;
        }
        // Delete budget by id for a specific user
        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var existing = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (existing == null) return false;

            _context.Budgets.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
