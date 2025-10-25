using FinTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Transaction = FinTrack.Domain.Entities.Transaction;

namespace FinTrack.Infrastructure.Data
{
    /// <summary>
    /// Application database context integrating ASP.NET Identity and domain entities.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain entities
        public DbSet<BankAccount> BankAccounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Budget> Budgets { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fluent API relationships & constraints
            builder.Entity<BankAccount>()
                .HasOne(b => b.User)
                .WithMany(u => u.BankAccounts)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Transaction>()
                .HasOne(t => t.BankAccount)
                .WithMany(b => b.Transactions)
                .HasForeignKey(t => t.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Budget>()
                .HasOne(b => b.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Budget>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            SeedInitialData(builder);
        }

        private void SeedInitialData(ModelBuilder builder)
        {
            // Default Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Groceries", ColorHex = "#FFB74D" },
                new Category { Id = 2, Name = "Utilities", ColorHex = "#4FC3F7" },
                new Category { Id = 3, Name = "Entertainment", ColorHex = "#BA68C8" },
                new Category { Id = 4, Name = "Transportation", ColorHex = "#81C784" },
                new Category { Id = 5, Name = "Salary", ColorHex = "#FFD54F" }
            );

            // Default Test User (optional)
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();
            var testUser = new ApplicationUser
            {
                Id = "seed-user-001",
                UserName = "demo@fintrack.com",
                NormalizedUserName = "DEMO@FINTRACK.COM",
                Email = "demo@fintrack.com",
                NormalizedEmail = "DEMO@FINTRACK.COM",
                EmailConfirmed = true,
                FirstName = "Demo",
                LastName = "User",
                PreferredCurrency = "USD",
                MonthlyIncome = 3000
            };
            testUser.PasswordHash = hasher.HashPassword(testUser, "Demo123!");

            builder.Entity<ApplicationUser>().HasData(testUser);

            // Default Bank Account for demo user
            builder.Entity<BankAccount>().HasData(
                new BankAccount
                {
                    Id = 1,
                    BankName = "Demo Bank",
                    AccountNumber = "DE12345678900000000001",
                    Balance = 1500,
                    UserId = testUser.Id
                }
            );

            // Default Transactions
            builder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = 1,
                    BankAccountId = 1,
                    Amount = 75,
                    Currency = "USD",
                    Date = DateTime.UtcNow.AddDays(-5),
                    Description = "Grocery shopping",
                    CategoryId = 1,
                    Type = "Expense"
                },
                new Transaction
                {
                    Id = 2,
                    BankAccountId = 1,
                    Amount = 2500,
                    Currency = "USD",
                    Date = DateTime.UtcNow.AddDays(-10),
                    Description = "Monthly salary",
                    CategoryId = 5,
                    Type = "Income"
                }
            );
        }
    }
}
