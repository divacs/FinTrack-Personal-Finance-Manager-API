using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities
{
    /// <summary>
    /// Represents a spending or income category.
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(7)]
        public string? ColorHex { get; set; } // Optional UI color code

        // Navigation
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
