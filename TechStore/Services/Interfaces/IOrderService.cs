using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(string userId, CheckoutViewModel model, Cart cart);

        Task<List<Order>> GetOrdersByUserIdAsync(string userId);

        Task<Order?> GetOrderByIdAsync(int orderId, string userId);

        Task<List<Order>> GetAllOrdersAsync();

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<PagedResultViewModel<Order>> GetPagedOrderAsync(string? search, string? status, int page, int pageSize);

        Task<int> GetOrderCountAsync(string? search, string? status);

        Task UpdateOrderStatusAsync(int orderId, string status);

        Task<int> GetOrderCountAsync();

        Task<decimal> GetTotalRevenueAsync();

    }
}
