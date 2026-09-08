using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;

namespace TechStore.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<List<WishlistItem>> GetByUserIdAsync(string userId)
        {
            return await _wishlistRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> IsInWishlistAsync(string userId, int productId)
        {
            return await _wishlistRepository.ExistsAsync(userId, productId);
        }

        public async Task AddAsync(string userId, int productId)
        {
            var exists = await _wishlistRepository.ExistsAsync(userId, productId);

            if (exists)
            {
                return;
            }

            var item = new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            };

            await _wishlistRepository.AddAsync(item);
            await _wishlistRepository.SaveAsync();
        }

        public async Task RemoveAsync(string userId, int productId)
        {
            var item = await _wishlistRepository.GetAsync(userId, productId);

            if (item == null)
            {
                return;
            }

            _wishlistRepository.Delete(item);
            await _wishlistRepository.SaveAsync();
        }
    }
}
