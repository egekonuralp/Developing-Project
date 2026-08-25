using TechStore.DTOs;
using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync(ProductFilterDto filter);

        Task<Product?> GetByIdAsync(int id);

        Task AddAsync(Product product);

        Task UpdateAsync(Product product);

        Task DeleteAsync(int id);

        Task<int> CountAsync();

        Task<List<Product>> GetRecentProductsAsync(int count);

        Task<List<CategoryProductCountDto>> GetProductCountByCategoryAsync();

        Task<int> CountAsync(ProductFilterDto filter);

        Task<List<Product>> GetActiveProductsAsync(ProductFilterDto filter);
    }
}
