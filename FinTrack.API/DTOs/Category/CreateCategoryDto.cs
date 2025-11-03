using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(7)]
        public string? ColorHex { get; set; }
    }
}
