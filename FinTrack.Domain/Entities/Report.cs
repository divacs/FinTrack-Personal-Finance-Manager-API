using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Domain.Entities
{
    /// <summary>
    /// Represents a monthly financial summary generated for the user.
    /// </summary>
    public class Report
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(7)]
        public string Month { get; set; } = null!; // Format: YYYY-MM

        [Required]
        public decimal TotalIncome { get; set; }

        [Required]
        public decimal TotalExpenses { get; set; }
        [NotMapped]
        public decimal NetBalance => TotalIncome - TotalExpenses;
    }
}
