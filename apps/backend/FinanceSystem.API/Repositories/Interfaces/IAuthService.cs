using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;

namespace FinanceSystem.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(User user, string token)> LoginAsync(LoginDto dto);
        Task<string> GenerateTokenAsync(User user);
        Task<User> FindOrCreateExternalUserAsync(string provider, string providerId, string email, string name);
        Task<(User user, string token)> LoginWithGoogleAsync(string code);
    }
}