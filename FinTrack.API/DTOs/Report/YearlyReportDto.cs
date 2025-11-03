namespace FinTrack.API.DTOs.Report
{
    public class YearlyReportDto
    {
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpenses;
    }
}
