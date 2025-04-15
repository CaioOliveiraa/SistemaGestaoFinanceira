using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;

namespace FinanceSystem.API.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(Guid userId)
        {
            return await _repo.GetAllAsync(userId);
        }

        public async Task<Category> CreateAsync(CategoryDto dto, Guid userId)
        {
            var category = new Category
            {
                Name = dto.Name,
                Type = dto.Type,
                Fixed = dto.Fixed,
                UserId = userId
            };
            
            await _repo.AddAsync(category);
            return category;
        }

        public async Task<Category?> UpdateAsync( Guid id,CategoryDto dto, Guid userId)
        {
            var category = await _repo.GetByIdAsync(id);

            if(category is null || category.UserId != userId)
            {
                return null;
            }

            category.Name = dto.Name;
            category.Type = dto.Type;
            category.Fixed = dto.Fixed;
            category.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(category);
            return category;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null || category.UserId != userId)
                return false;

            await _repo.DeleteAsync(category);
            return true;
        }
    }
}