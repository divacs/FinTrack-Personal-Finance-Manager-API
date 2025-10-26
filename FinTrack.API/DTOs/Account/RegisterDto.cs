using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTOs.Account
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [MaxLength(10)]
        public string? PreferredCurrency { get; set; }

        public decimal? MonthlyIncome { get; set; }
    }
}
