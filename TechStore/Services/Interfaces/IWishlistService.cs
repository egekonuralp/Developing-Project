using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<List<WishlistItem>> GetByUserIdAsync(string userId);

        Task<bool> IsInWishlistAsync(string userId, int productId);

        Task AddAsync(string userId, int productId);

        Task RemoveAsync(string userId, int productId);
    }
}
