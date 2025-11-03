namespace FinTrack.API.DTOs.Budget
{
    public class GetBudgetDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal LimitAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? CurrentSpending { get; set; }
    }
}
