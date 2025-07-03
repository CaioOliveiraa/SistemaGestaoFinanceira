using System.Runtime.InteropServices;
using FinanceSystem.API.Repositories.Interfaces;

namespace FinanceSystem.API.Services
{
    public class DashboardService
    {
        private readonly ITransactionRepository _repo;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ITransactionRepository repo, ILogger<DashboardService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<object> GetSymmaryAsync(Guid userId)
        {
            var transactions = await _repo.GetAllByUserIdAsync(userId);

            var income = transactions
                .Where(t => t.Type == Models.TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = transactions
                .Where(t => t.Type == Models.TransactionType.Expense)
                .Sum(t => t.Amount);

            return new
            {
                Income = income,
                Expense = expense,
                Balance = income - expense
            };
        }

        public async Task<IEnumerable<object>> GetMonthlyAsync(Guid userId)
        {
            var transactions = await _repo.GetAllByUserIdAsync(userId);

            var grouped = transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderByDescending(t => t.Key.Year * 100 + t.Key.Month) // Gera uma ordenação numerica no estilo YYYYMM
                .Take(6)
                .Select(t =>
                {
                    var income = t.Where(t => t.Type == Models.TransactionType.Income).Sum(t => t.Amount);
                    var expense = t.Where(t => t.Type == Models.TransactionType.Expense).Sum(t => t.Amount);

                    return new
                    {
                        Month = $"{t.Key.Month}/{t.Key.Year}",
                        Income = income,
                        Expense = expense,
                        Balance = income - expense
                    };
                })
                .OrderBy(x => x.Month);

            return grouped.ToList();
        }

        public async Task<IEnumerable<object>> GetByCategoryAsync(Guid userId)
        {
            var transactions = await _repo.GetAllByUserIdAsync(userId);

            var grouped = transactions
                .Where(t => t.Date.Year == DateTime.UtcNow.Year && t.Date.Month == DateTime.UtcNow.Month)
                .ToList();

            var result = grouped
                .GroupBy(t => new { t.Category.Name, t.Category.Type })
                .Select(g => new
                {
                    Category = g.Key.Name,
                    Type = g.Key.Type.ToString(),
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<object>> GetDailyAsync(Guid userId)
        {
            var transactions = await _repo.GetAllByUserIdAsync(userId);
            var startDate = DateTime.UtcNow.AddDays(-30);
            var recent = transactions.Where(t => t.Date >= startDate);


            var grouped = recent
                .GroupBy(t => t.Date.Date)
                .Select(t =>
                {
                    var income = t.Where(t => t.Type == Models.TransactionType.Income).Sum(t => t.Amount);
                    var expense = t.Where(t => t.Type == Models.TransactionType.Expense).Sum(t => t.Amount);

                    return new
                    {
                        Date = t.Key.ToString("dd/MM/yyyy"),
                        Income = income,
                        Expense = expense
                    };
                })
                .OrderBy(g => g.Date)
                .ToList();

            return grouped;
        }
    }
}