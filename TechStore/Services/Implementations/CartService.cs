using TechStore.Data;
using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TechStore.ViewModels;

namespace TechStore.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new InvalidOperationException("Ürün Bulunamadı.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException(
                    $"{product.Name} ürünü artık satışta değil.");
            }

            if (product.Stock <= 0)
            {
                throw new InvalidOperationException("Ürün Stokta Bulunmamaktadır.");
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException("Geçersiz Ürün Miktarı.");
            }

            if (quantity > product.Stock)
            {
                throw new InvalidOperationException(
                    "Stokta Eklemek İstediğiniz Kadar Ürün Bulunmamaktadır.");
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };

                await _cartRepository.AddAsync(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                var newQuantity = cartItem.Quantity + quantity;

                if (newQuantity > product.Stock)
                {
                    throw new InvalidOperationException(
                        $"Bu üründen en fazla {product.Stock} adet satın alabilirsiniz.");
                }

                cartItem.Quantity = newQuantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    Cart = cart,
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }

            await _cartRepository.SaveAsync();
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart != null && SyncCartItemPrices(cart))
            {
                await _cartRepository.SaveAsync();
            }

            return cart;
        }

        private static bool SyncCartItemPrices(Cart cart)
        {
            var pricesChanged = false;

            foreach (var cartItem in cart.CartItems)
            {
                if (cartItem.Product != null && cartItem.UnitPrice != cartItem.Product.Price)
                {
                    cartItem.UnitPrice = cartItem.Product.Price;
                    pricesChanged = true;
                }
            }

            return pricesChanged;
        }

        public async Task IncreaseQuantityAsync(string userId, int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId, userId);

            if (cartItem == null)
            {
                throw new Exception("Sepet Ürünü Bulunamadı.");
            }

            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new Exception("Ürün Bulunamadı.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException(
                    $"{product.Name} ürünü artık satışta değil.");
            }

            if (cartItem.Quantity >= product.Stock)
            {
                throw new Exception($"Bu üründen en fazla {product.Stock} adet satın alabilirsiniz.");
            }

            cartItem.Quantity++;

            await _cartRepository.SaveAsync();
        }

        public async Task DecreaseQuantityAsync(string userId, int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId, userId);

            if (cartItem == null)
            {
                throw new Exception("Sepet Ürünü Bulunamadı.");
            }

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;

                await _cartRepository.SaveAsync();
            }
        }

        public async Task RemoveFromCartAsync(string userId, int cartItemID)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemID, userId);

            if (cartItem == null)
            {
                throw new Exception("Sepet Ürünü Bulunamadı.");
            }

            await _cartRepository.RemoveCartItemAsync(cartItem);
        }

        public async Task<CartIndexViewModel> GetCartSummaryAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return new CartIndexViewModel
                {
                    Cart = new Cart(),
                    TotalPrice = 0,
                    TotalQuantity = 0
                };
            }

            return new CartIndexViewModel
            {
                Cart = cart,

                TotalPrice = cart.CartItems.Sum(x => x.TotalPrice),

                TotalQuantity = cart.CartItems.Sum(x => x.Quantity)
            };
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return;
            }

            await _cartRepository.ClearCartAsync(cart);
        }
    }
}