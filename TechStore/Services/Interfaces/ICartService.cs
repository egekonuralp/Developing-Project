using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int productId, int quantity);

        Task<Cart?> GetCartByUserIdAsync(string userId);

        Task IncreaseQuantityAsync(string userId, int cartItemId);

        Task DecreaseQuantityAsync(string userId, int cartItemId);

        Task RemoveFromCartAsync(string userId, int cartItemId);
        
        Task<CartIndexViewModel> GetCartSummaryAsync(string userId);

        Task ClearCartAsync(string userId);
    }
}
