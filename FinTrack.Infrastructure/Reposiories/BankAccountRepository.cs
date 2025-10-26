using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using FinTrack.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FinTrack.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public BankAccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankAccount>> GetAllAsync(string userId, bool allUsers = false)
        {
            if (allUsers)
                return await _context.BankAccounts.ToListAsync();

            return await _context.BankAccounts
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<BankAccount?> GetByIdAsync(int id, string userId, bool allUsers = false)
        {
            if (allUsers)
                return await _context.BankAccounts.FirstOrDefaultAsync(b => b.Id == id);

            return await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }

        public async Task<BankAccount> AddAsync(BankAccount account)
        {
            _context.BankAccounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<BankAccount?> UpdateAsync(BankAccount account)
        {
            var existing = await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.Id == account.Id && (account.UserId == b.UserId));

            if (existing == null) return null;

            existing.BankName = account.BankName;
            existing.AccountNumber = account.AccountNumber;
            existing.Balance = account.Balance;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool allUsers = false)
        {
            BankAccount? existing;
            if (allUsers)
                existing = await _context.BankAccounts.FirstOrDefaultAsync(b => b.Id == id);
            else
                existing = await _context.BankAccounts.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (existing == null) return false;

            _context.BankAccounts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
