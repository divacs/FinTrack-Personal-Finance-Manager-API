using FinTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Transaction = FinTrack.Domain.Entities.Transaction;

namespace FinTrack.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Budget> Budgets { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ------------------------
            // Seed Users
            // ------------------------
            var hasher = new PasswordHasher<ApplicationUser>();
            var demoUser = new ApplicationUser
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
            demoUser.PasswordHash = hasher.HashPassword(demoUser, "Demo123!");
            builder.Entity<ApplicationUser>().HasData(demoUser);

            // ------------------------
            // Seed Categories
            // ------------------------
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Groceries", ColorHex = "#FFB74D" },
                new Category { Id = 2, Name = "Utilities", ColorHex = "#4FC3F7" },
                new Category { Id = 3, Name = "Entertainment", ColorHex = "#BA68C8" },
                new Category { Id = 4, Name = "Transportation", ColorHex = "#81C784" },
                new Category { Id = 5, Name = "Salary", ColorHex = "#FFD54F" }
            );

            // ------------------------
            // Seed BankAccount
            // ------------------------
            builder.Entity<BankAccount>().HasData(
                new BankAccount
                {
                    Id = 1,
                    BankName = "Demo Bank",
                    AccountNumber = "DE12345678900000000001",
                    Balance = 1500,
                    UserId = demoUser.Id
                }
            );

            // ------------------------
            // Seed Transactions (FK: BankAccountId, CategoryId)
            // ------------------------
            builder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = 1,
                    BankAccountId = 1,
                    Amount = 75.00m,
                    Currency = "USD",
                    Date = new DateTime(2025, 05, 01),
                    Description = "Grocery shopping",
                    CategoryId = 1,
                    Type = "Expense"
                },
                new Transaction
                {
                    Id = 2,
                    BankAccountId = 1,
                    Amount = 2500.00m,
                    Currency = "USD",
                    Date = new DateTime(2025, 04, 20),
                    Description = "Monthly salary",
                    CategoryId = 5,
                    Type = "Income"
                }
            );

            // ------------------------
            // Seed Budgets (FK: UserId, CategoryId)
            // ------------------------
            builder.Entity<Budget>().HasData(
                new Budget
                {
                    Id = 1,
                    UserId = demoUser.Id,
                    CategoryId = 1,
                    LimitAmount = 500.00m
                },
                new Budget
                {
                    Id = 2,
                    UserId = demoUser.Id,
                    CategoryId = 2,
                    LimitAmount = 200.00m
                }
            );

            // ------------------------
            // Seed Reports (FK: UserId)
            // ------------------------
            builder.Entity<Report>().HasData(
                new Report
                {
                    Id = 1,
                    UserId = demoUser.Id,
                    TotalIncome = 2500.00m,
                    TotalExpenses = 75.00m,
                    Month = "5"
    
                }
            );

            // ------------------------
            // Configure Decimal Precision
            // ------------------------
            builder.Entity<Budget>()
                .Property(b => b.LimitAmount)
                .HasPrecision(18, 2);

            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Report>()
                .Property(r => r.TotalIncome)
                .HasPrecision(18, 2);

            builder.Entity<Report>()
                .Property(r => r.TotalExpenses)
                .HasPrecision(18, 2);

            // ------------------------
            // Fluent API Relationships
            // ------------------------
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
        }
    }
}
