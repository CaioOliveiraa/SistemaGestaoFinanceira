using BCrypt.Net;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;

namespace FinanceSystem.API.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            var existingUser = await _repo.GetByEmailAsync(dto.Email);

            if (existingUser is not null)
            {
                throw new Exception("User already exists");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User()
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Type = UserType.Common
            };

            await _repo.AddAsync(user);
            return user;

        }
        
    }
}