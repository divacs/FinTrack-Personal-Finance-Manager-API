using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTOs.Budget
{
    public class UpdateBudgetDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal LimitAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
