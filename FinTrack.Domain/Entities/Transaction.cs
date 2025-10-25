using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities
{
    /// <summary>
    /// Represents a financial transaction associated with a bank account.
    /// </summary>
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required, MaxLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Link to category
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Type: Income or Expense
        [Required, MaxLength(10)]
        public string Type { get; set; } = "Expense";
    }
}
