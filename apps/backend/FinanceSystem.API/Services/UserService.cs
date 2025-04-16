using AutoMapper;
using BCrypt.Net;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;

namespace FinanceSystem.API.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            var existingUser = await _repo.GetByEmailAsync(dto.Email);

            if (existingUser is not null)
            {
                throw new Exception("User already exists");
            }

            var user = _mapper.Map<User>(dto); // Mapeie de createUserDto para um objeto do tipo User
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Type = UserType.Common;

            await _repo.AddAsync(user);
            return _mapper.Map<UserResponseDto>(user);
        }
        
    }
}