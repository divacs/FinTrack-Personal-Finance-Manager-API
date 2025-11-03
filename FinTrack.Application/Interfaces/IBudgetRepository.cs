using FinTrack.Domain.Entities;

namespace FinTrack.Application.Interfaces
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<Budget>> GetUserBudgetsAsync(string userId);
        Task<Budget?> GetByIdAsync(int id, string userId);
        Task<Budget> AddAsync(Budget budget);
        Task<Budget?> UpdateAsync(Budget budget);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
