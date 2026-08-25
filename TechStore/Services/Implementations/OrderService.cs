using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task CreateOrderAsync(string userId, CheckoutViewModel model, Cart cart)
        {
            var products = new Dictionary<int, Product>();

            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);

                if (product == null)
                {
                    throw new Exception($"{cartItem.ProductName} ürünü bulunamadı.");
                }

                if (product.Stock < cartItem.Quantity)
                {
                    throw new Exception($"{product.Name} ürününden yeterli stok bulunmamaktadır. " +
                                        $"Mevcut stok: {product.Stock}");
                }

                products[cartItem.ProductId] = product;
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
                Status = "Hazırlanıyor"
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

                products[cartItem.ProductId].Stock -= cartItem.Quantity;
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveAsync();
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

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return;
            }

            order.Status = status;

            await _orderRepository.SaveAsync();
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