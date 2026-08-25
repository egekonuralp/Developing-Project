using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task AddAsync(Category category);

        void Update(Category category);

        void Delete(Category category);

        Task SaveAsync();

        Task<int> CountAsync();

        Task<List<Category>> GetRecentCategoriesAsync(int count);
    }
}