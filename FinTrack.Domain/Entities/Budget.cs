using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities
{
    /// <summary>
    /// Represents a user's spending budget for a specific category and period.
    /// </summary>
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal LimitAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Computed field for tracking current spending
        public decimal? CurrentSpending { get; set; }
    }
}
