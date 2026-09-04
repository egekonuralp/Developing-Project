using TechStore.Models;
using TechStore.Data;
using Microsoft.EntityFrameworkCore;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ApplicationDbContext _context;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _context = context;
        }

        public async Task CreateOrderAsync(string userId, CheckoutViewModel model, Cart cart)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(x => x.Id == cartItem.ProductId);

                    if (product == null)
                    {
                        throw new InvalidOperationException(
                            $"{cartItem.ProductName} ürünü artık mevcut değil.");
                    }

                    if (!product.IsActive)
                    {
                        throw new InvalidOperationException(
                            $"{cartItem.ProductName} ürünü artık satışta değil.");
                    }
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalPrice = cart.CartItems.Sum(x => x.UnitPrice * x.Quantity),
                    FullName = model.FullName,
                    Phone = model.PhoneNumber,
                    City = model.City,
                    District = model.District,
                    Address = model.Address,
                    Status = OrderStatuses.Preparing
                };

                foreach (var cartItem in cart.CartItems)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.ProductName,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice,
                    });

                }

                foreach (var productGroup in cart.CartItems.GroupBy(item => item.ProductId))
                {
                    var quantity = productGroup.Sum(item => item.Quantity);
                    var productName = productGroup.First().ProductName;

                    var affectedRows = await _context.Products
                        .Where(product => product.Id == productGroup.Key && product.Stock >= quantity)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(product => product.Stock, product => product.Stock - quantity));

                    if (affectedRows == 0)
                    {
                        throw new InvalidOperationException(
                            $"{productName} ürünü için yeterli stok bulunmamaktadır.");
                    }
                }

                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveAsync();
                await _cartRepository.ClearCartAsync(cart);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId, string userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return null;
            }

            if (order.UserId != userId)
            {
                return null;
            }

            return order;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<PagedResultViewModel<Order>> GetPagedOrderAsync(string? search, string? status, int page, int pageSize)
        {
            var orders = await _orderRepository.GetPagedOrderAsync(search, status, page, pageSize);

            var totalCount = await _orderRepository.GetOrderCountAsync(search, status);

            return new PagedResultViewModel<Order>
            {
                Items = orders,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Search = search
            };
        }

        public async Task<int> GetOrderCountAsync(string? search, string? status)
        {
            return await _orderRepository.GetOrderCountAsync(search, status);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            if (!OrderStatuses.IsValid(status))
            {
                throw new ArgumentException("Geçersiz sipariş durumu.", nameof(status));
            }

            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            order.Status = status;

            await _orderRepository.SaveAsync();
            return true;
        }

        public async Task<int> GetOrderCountAsync()
        {
            return await _orderRepository.GetOrderCountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _orderRepository.GetTotalRevenueAsync();
        }
    }
}
