using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int productId, int quantity);

        Task<Cart?> GetCartByUserIdAsync(string userId);

        Task IncreaseQuantityAsync(int cartItemId);

        Task DecreaseQuantityAsync(int cartItemId);

        Task RemoveFromCartAsync(int cartItemId);
        
        Task<CartIndexViewModel> GetCartSummaryAsync(string userId);

        Task ClearCartAsync(string userId);
    }
}