using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all transactions for a user or all users if specified
        public async Task<IEnumerable<Transaction>> GetAllAsync(string userId, bool allUsers)
        {
            var query = _context.Transactions.Include(t => t.BankAccount)
                                             .Include(t => t.Category)
                                             .AsQueryable();

            if (!allUsers)
                query = query.Where(t => t.BankAccount.UserId == userId);

            return await query.ToListAsync();
        }
        // Get a transaction by ID, ensuring it belongs to the user unless allUsers is true
        public async Task<Transaction?> GetByIdAsync(int id, string userId, bool allUsers)
        {
            var query = _context.Transactions.Include(t => t.BankAccount)
                                             .Include(t => t.Category)
                                             .AsQueryable();

            if (!allUsers)
                query = query.Where(t => t.BankAccount.UserId == userId);

            return await query.FirstOrDefaultAsync(t => t.Id == id);
        }
        // Add a new transaction
        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
        // Update an existing transaction
        public async Task<Transaction?> UpdateAsync(Transaction transaction)
        {
            var existing = await _context.Transactions.FindAsync(transaction.Id);
            if (existing == null) return null;

            existing.Amount = transaction.Amount;
            existing.Currency = transaction.Currency;
            existing.Date = transaction.Date;
            existing.Description = transaction.Description;
            existing.CategoryId = transaction.CategoryId;
            existing.Type = transaction.Type;

            await _context.SaveChangesAsync();
            return existing;
        }
        // Delete a transaction by ID, ensuring it belongs to the user unless allUsers is true
        public async Task<bool> DeleteAsync(int id, string userId, bool allUsers)
        {
            var query = _context.Transactions.Include(t => t.BankAccount).AsQueryable();

            if (!allUsers)
                query = query.Where(t => t.BankAccount.UserId == userId);

            var transaction = await query.FirstOrDefaultAsync(t => t.Id == id);
            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
        // Generate a report based on filters
        public async Task<IEnumerable<Transaction>> GetReportAsync(string userId, bool allUsers, DateTime? fromDate, DateTime? toDate, int? categoryId, int? bankAccountId)
        {
            var query = _context.Transactions.Include(t => t.BankAccount)
                                             .Include(t => t.Category)
                                             .AsQueryable();

            if (!allUsers)
                query = query.Where(t => t.BankAccount.UserId == userId);

            if (fromDate.HasValue) query = query.Where(t => t.Date >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(t => t.Date <= toDate.Value);
            if (categoryId.HasValue) query = query.Where(t => t.CategoryId == categoryId.Value);
            if (bankAccountId.HasValue) query = query.Where(t => t.BankAccountId == bankAccountId.Value);

            return await query.ToListAsync();
        }
    }
}
