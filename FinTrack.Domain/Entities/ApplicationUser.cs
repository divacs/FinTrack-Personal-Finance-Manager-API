using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities
{
    /// <summary>
    /// Represents an application user extending IdentityUser with domain-specific fields.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [MaxLength(10)]
        public string? PreferredCurrency { get; set; }

        public decimal? MonthlyIncome { get; set; }

        // Navigation properties
        public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
