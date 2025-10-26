using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTOs.BankAccount
{
    public class GetBankAccountDto
    {
        public int Id { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
