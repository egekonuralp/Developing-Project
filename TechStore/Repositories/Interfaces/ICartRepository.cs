using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);

        Task AddAsync(Cart cart);

        Task AddCartItemAsync(CartItem cartItem);

        Task SaveAsync();

        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);

        Task RemoveCartItemAsync(CartItem cartItem);

        Task ClearCartAsync(Cart cart);
    }
}
