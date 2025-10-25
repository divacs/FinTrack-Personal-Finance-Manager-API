using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities
{
    /// <summary>
    /// Represents a user's bank account with linked transactions.
    /// </summary>
    public class BankAccount
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string BankName { get; set; } = null!;

        [Required, MaxLength(34)] // IBAN max length
        public string AccountNumber { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }

        // Foreign key to user
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        // Navigation
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
