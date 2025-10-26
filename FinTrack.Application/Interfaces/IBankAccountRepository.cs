using FinTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Application.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAllAsync(string userId, bool allUsers = false); // allUsers = true -> Admin can see all accounts
        Task<BankAccount?> GetByIdAsync(int id, string userId, bool allUsers = false); // allUsers = true -> Admin can see any account
        Task<BankAccount> AddAsync(BankAccount account);
        Task<BankAccount?> UpdateAsync(BankAccount account);
        Task<bool> DeleteAsync(int id, string userId, bool allUsers = false);
    }
}
