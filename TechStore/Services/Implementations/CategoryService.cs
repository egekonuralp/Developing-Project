using TechStore.Models;
using TechStore.Repositories.Implementations;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;

namespace TechStore.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new Exception("Kategori adı boş olamaz.");
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new Exception("Kategori adı boş olamaz.");
            }

            _categoryRepository.Update(category);
            await _categoryRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                throw new Exception("Kategori bulunamadı.");
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _categoryRepository.CountAsync();
        }

        public async Task<List<Category>> GetRecentCategoriesAsync(int count)
        {
            return await _categoryRepository.GetRecentCategoriesAsync(count);
        }
    }
}