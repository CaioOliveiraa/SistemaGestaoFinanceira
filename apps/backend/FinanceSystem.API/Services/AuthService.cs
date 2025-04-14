using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;
using FinanceSystem.API.Helpers;

namespace FinanceSystem.API.Services
{
    public class AuthService
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config= config;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var secret = Environment.GetEnvironmentVariable("JwtSecret");

            if (string.IsNullOrEmpty(secret)) throw new Exception("JWT secret not found in configuration.");

            return JwtHelper.GenerateJwtToken(user, secret);
        }
    }
}