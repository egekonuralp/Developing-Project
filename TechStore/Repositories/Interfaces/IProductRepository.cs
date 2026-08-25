using TechStore.DTOs;
using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(ProductFilterDto filter);

        Task<Product?> GetByIdAsync(int id);

        Task AddAsync(Product product);

        void Update(Product product);

        void Delete(Product product);

        Task SaveAsync();

        Task<int> CountAsync();

        Task<List<Product>> GetRecentProductsAsync(int count);

        Task<List<CategoryProductCountDto>> GetProductCountByCategoryAsync();

        Task<int> CountAsync(ProductFilterDto filter);

        Task<List<Product>> GetActiveProductsAsync(ProductFilterDto filter);
    }
}