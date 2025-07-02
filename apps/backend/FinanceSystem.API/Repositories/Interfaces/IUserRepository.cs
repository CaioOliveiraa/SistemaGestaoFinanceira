using FinanceSystem.API.Models;

namespace FinanceSystem.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task<User?> GetByExternalIdAsync(string provider, string providerId);
    }
}