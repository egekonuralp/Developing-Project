using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetByProductIdAsync(int productId);

        Task<ProductImage?> GetByIdAsync(int id);

        Task AddAsync(ProductImage image);

        void Delete(ProductImage image);

        Task SaveAsync();
    }
}
