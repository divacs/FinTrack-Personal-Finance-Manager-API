using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTOs.Account
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }
}
