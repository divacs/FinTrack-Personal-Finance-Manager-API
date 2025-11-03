namespace FinTrack.API.DTOs.Category
{
    public class GetCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ColorHex { get; set; }
    }
}
