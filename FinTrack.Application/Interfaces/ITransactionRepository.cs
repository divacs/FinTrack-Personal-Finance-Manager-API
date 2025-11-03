using FinTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync(string userId, bool allUsers);
        Task<Transaction?> GetByIdAsync(int id, string userId, bool allUsers);
        Task<Transaction> AddAsync(Transaction transaction);
        Task<Transaction?> UpdateAsync(Transaction transaction);
        Task<bool> DeleteAsync(int id, string userId, bool allUsers);
        Task<IEnumerable<Transaction>> GetReportAsync(string userId, bool allUsers, DateTime? fromDate, DateTime? toDate, int? categoryId, int? bankAccountId);

    }
}
