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
    }
}