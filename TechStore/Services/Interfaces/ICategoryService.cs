using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task AddAsync(Category category);

        Task UpdateAsync(Category category);

        Task DeleteAsync(int id);

        Task<int> CountAsync();

        Task<List<Category>> GetRecentCategoriesAsync(int count);
    }
}
