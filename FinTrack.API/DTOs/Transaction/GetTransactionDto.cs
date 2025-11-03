namespace FinTrack.API.DTOs.Transaction
{
    public class GetTransactionDto
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Type { get; set; } = null!;
    }
}
