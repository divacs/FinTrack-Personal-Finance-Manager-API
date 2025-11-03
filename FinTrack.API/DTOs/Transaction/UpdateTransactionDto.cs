using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTOs.Transaction
{
    public class UpdateTransactionDto
    {
        [Required]
        public int BankAccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Type { get; set; } = "Expense";
    }
}
