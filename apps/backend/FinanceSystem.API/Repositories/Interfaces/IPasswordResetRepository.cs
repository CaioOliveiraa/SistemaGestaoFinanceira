using FinanceSystem.API.Models;

namespace FinanceSystem.API.Repositories.Interfaces
{
    public interface IPasswordResetRepository
    {
        public Task<PasswordReset?> GetByUserIdAsync(Guid userId);
        public Task<PasswordReset?> GetByTokenAsync(string token);
        Task AddAsync(PasswordReset passwordReset);
        Task UpdateAsync(PasswordReset passwordReset);
        Task DeleteAsync(PasswordReset passwordReset);
    }
}