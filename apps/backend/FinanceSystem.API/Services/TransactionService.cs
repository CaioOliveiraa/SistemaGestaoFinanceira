using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;

namespace FinanceSystem.API.Services
{
    public class TransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync(Guid userId)
        {
            return await _repo.GetAllByUserIdAsync(userId);
        }

        public async Task<Transaction> CreateAsync(TransactionDto dto, Guid userId)
        {
            var Transaction = new Transaction
            {
                Type = dto.Type,
                Amount = dto.Amount,
                Description = dto.Description,
                Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                Recurring = dto.Recurring,
                CategoryId = dto.CategoryId,
                UserId = userId
            };

            await _repo.AddAsync(Transaction);
            return Transaction;
        }

        public async Task<Transaction?> UpdateAsync(Guid id, TransactionDto dto, Guid userId)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction is null || transaction.UserId != userId)
                return null;

            transaction.Type = dto.Type;
            transaction.Amount = dto.Amount;
            transaction.Description = dto.Description;
            transaction.Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);
            transaction.Recurring = dto.Recurring;
            transaction.CategoryId = dto.CategoryId;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(transaction);
            return transaction;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction is null || transaction.UserId != userId)
                return false;

            await _repo.DeleteAsync(transaction);
            return true;
        }
    }
}