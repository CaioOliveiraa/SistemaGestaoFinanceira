namespace FinanceSystem.API.Dtos
{
    public class UserMonthlySummary
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;
    }
}
