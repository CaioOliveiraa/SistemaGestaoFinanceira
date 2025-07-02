public interface IReportService
{
    Task<IEnumerable<UserMonthlySummary>> GenerateMonthlyReportAsync(DateTime month);
}

public class UserMonthlySummary
{
    public string Email { get; set; } = "";
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
}