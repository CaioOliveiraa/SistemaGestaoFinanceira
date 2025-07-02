using FinanceSystem.API.Dtos;
using FinanceSystem.API.Repositories.Interfaces;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;

    public ReportService(IReportRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<UserMonthlySummary>> GenerateMonthlyReportAsync(DateTime date)
    {
        return _repo.GenerateMonthlyReportAsync(date);
    }
}
