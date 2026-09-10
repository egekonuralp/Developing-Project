using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface IProductImageService
    {
        Task<List<ProductImage>> GetByProductIdAsync(int productId);

        Task<ProductImage?> GetByIdAsync(int Id);

        Task AddImageAsync(int productId, string imageUrl);

        Task DeleteImageAsync(int imageId);

        Task SetMainImageAsync(int productId, int imageId);
    }
}
