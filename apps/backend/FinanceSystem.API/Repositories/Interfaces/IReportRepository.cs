using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;

namespace FinanceSystem.API.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<UserMonthlySummary>> GenerateMonthlyReportAsync(DateTime date);
    }
}