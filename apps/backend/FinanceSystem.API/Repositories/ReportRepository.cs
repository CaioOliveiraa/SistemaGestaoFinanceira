using FinanceSystem.API.Data;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly FinanceDbContext _context;

        public ReportRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserMonthlySummary>> GenerateMonthlyReportAsync(DateTime date)
        {
            var month = date.Month;
            var year = date.Year;

            // Agrupa transações por usuário
            var query = from tx in _context.Transactions
                        where tx.Date.Month == month && tx.Date.Year == year
                        group tx by tx.UserId into g
                        select new
                        {
                            UserId = g.Key,
                            Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                            Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                        };

            var summaries = await query.ToListAsync();

            // Pega emails correspondentes
            var users = await _context.Users
                .Where(u => summaries.Select(s => s.UserId).Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            return summaries.Select(s => new UserMonthlySummary
            {
                Email = users[s.UserId],
                Month = month,
                Year = year,
                TotalIncome = s.Income,
                TotalExpense = s.Expense
            });
        }
    }
}
