using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Domain.Entities
{
    // Represents a record of a background report email job (monthly or yearly).
    public class ReportJobLogs
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required, MaxLength(10)]
        public string ReportType { get; set; } = null!; // "Monthly" or "Yearly"

        [MaxLength(7)]
        public string? Period { get; set; } // e.g., "2025-10" or "2025"

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsSuccess { get; set; } = false;

        [MaxLength(500)]
        public string? ErrorMessage { get; set; }

        [MaxLength(100)]
        public string? HangfireJobId { get; set; }
    }
}
