using FinanceSystem.API.Data;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FinanceDbContext _context;
        public UserRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByExternalIdAsync(string provider, string providerId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.OAuthProvider == provider &&
                    u.OauthId == providerId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}