using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTOs.BankAccount
{
    public class CreateBankAccountDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string BankName { get; set; } = null!;

        [Required]
        [StringLength(34, MinimumLength = 5)] 
        public string AccountNumber { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative")]
        public decimal Balance { get; set; }
    }
}
