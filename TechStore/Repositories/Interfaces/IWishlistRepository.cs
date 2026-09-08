using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<List<WishlistItem>> GetByUserIdAsync(string userId);

        Task<bool> ExistsAsync(string userId, int productId);

        Task AddAsync(WishlistItem item);

        void Delete(WishlistItem item);

        Task<WishlistItem?> GetAsync(string userId, int productId);

        Task SaveAsync();
    }
}
