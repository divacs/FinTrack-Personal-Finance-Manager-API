namespace FinTrack.API.DTOs.Report
{
    public class MonthlyReportDto
    {
        public string Month { get; set; } = null!; // Format YYYY-MM
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpenses;
    }
}
