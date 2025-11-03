namespace FinTrack.API.DTOs.Transaction
{
    public class TransactionReportDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CategoryId { get; set; }
        public int? BankAccountId { get; set; }
    }
}
