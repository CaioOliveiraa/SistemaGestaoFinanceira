using System;
using System.Threading.Tasks;
using FinanceSystem.API.Data;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.API.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly FinanceDbContext _context;

        public PasswordResetRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public Task<PasswordReset?> GetByUserIdAsync(Guid userId)
        {
            return _context.PasswordResets.FirstOrDefaultAsync(rp => rp.UserId == userId);
        }

        public Task<PasswordReset?> GetByTokenAsync(string token)
        {
            return _context.PasswordResets.FirstOrDefaultAsync(rp => rp.Token == token);
        }

        public Task AddAsync(PasswordReset passwordReset)
        {
            _context.PasswordResets.Add(passwordReset);
            return _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PasswordReset passwordReset)
        {
            _context.PasswordResets.Update(passwordReset);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PasswordReset passwordReset)
        {
            _context.PasswordResets.Remove(passwordReset);
            await _context.SaveChangesAsync();
        }
    }
}