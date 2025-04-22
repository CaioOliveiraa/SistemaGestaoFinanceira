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
    }
}