using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);

        Task SaveAsync();

        Task<List<Order>> GetOrdersByUserIdAsync(string userId);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<List<Order>> GetAllOrdersAsync();

        Task<List<Order>> GetPagedOrderAsync(string? search, string? status, int page, int pageSize);

        Task<int> GetOrderCountAsync(string? search, string? status);

        Task<int> GetOrderCountAsync();

        Task<decimal> GetTotalRevenueAsync();
    }
}
