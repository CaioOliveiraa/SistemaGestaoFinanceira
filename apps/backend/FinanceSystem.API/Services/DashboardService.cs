using FinanceSystem.API.Repositories.Interfaces;

namespace FinanceSystem.API.Services
{
    public class DashboardService
    {
        private readonly ITransactionRepository _repo;

        public DashboardService(ITransactionRepository repo)
        {
            _repo = repo;
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
                .GroupBy(t => new { t.Date.Year, t.Date.Month})
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
                .Where(t => t.Date.Year == DateTime.UtcNow.Year && t.Date.Month == DateTime.UtcNow.Month);

            var result = grouped
                .GroupBy( t => new { t.Category.Name, t.Category.Type })
                .Select(t => new
                {
                    Category = t.Key.Name,
                    Type = t.Key.Type.ToString(),
                    Amount = t.Sum(t => t.Amount)
                })
                .OrderByDescending(g => g.Amount)
                .ToList();

            return result;
        }
    }
}