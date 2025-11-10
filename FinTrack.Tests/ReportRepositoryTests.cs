using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Repositories;
using FinTrack.Domain.Entities;

public class ReportRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;

    public ReportRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        // seed user and categories, transactions
        var user = new ApplicationUser { Id = "u1", UserName = "u1", Email = "u1@x.com" };
        _context.Users.Add(user);

        var catIncome = new Category { Id = 1, Name = "Salary" };
        var catExpense = new Category { Id = 2, Name = "Groceries" };
        _context.Categories.AddRange(catIncome, catExpense);

        _context.Transactions.AddRange(
            new Transaction { Id = 1, BankAccountId = 0, Amount = 1000m, Date = new DateTime(2025, 11, 5), CategoryId = 1, Type = "Income" },
            new Transaction { Id = 2, BankAccountId = 0, Amount = 200m, Date = new DateTime(2025, 11, 10), CategoryId = 2, Type = "Expense" }
        );

        _context.SaveChanges();
    }

    [Fact]
    public async Task GenerateMonthlyReportAsync_CalculatesSumsCorrectly()
    {
        // Arrange
        var repo = new ReportRepository(_context);

        // Act
        var result = await repo.GenerateMonthlyReportAsync("u1", 2025, 11);

        // Assert
        result.TotalIncome.Should().Be(1000m);
        result.TotalExpenses.Should().Be(200m);
    }

    public void Dispose()
    {
        _context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
