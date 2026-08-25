using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public OrderController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                Cart = cart,
                TotalQuantity = cart.CartItems.Sum(x => x.Quantity),
                TotalPrice = cart.CartItems.Sum(x => x.Quantity * x.UnitPrice)
            };

            return View(viewModel);
        }  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cart = await _cartService.GetCartByUserIdAsync(userId!);

                model.Cart = cart!;
                model.TotalQuantity = cart!.CartItems.Sum(x => x.Quantity);
                model.TotalPrice = cart.CartItems.Sum(x => x.Quantity * x.UnitPrice);

                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            var currentCart = await _cartService.GetCartByUserIdAsync(currentUserId);

            if (currentCart == null || !currentCart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction("Payment", new
            {
                fullName = model.FullName,
                phoneNumber = model.PhoneNumber,
                city = model.City,
                district = model.District,
                address = model.Address,
            });
        }

        [HttpGet]
        public async Task<IActionResult> Payment(string fullName, string phoneNumber, string city, string  district, string address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new PaymentViewModel
            {
                Cart = cart,
                TotalQuantity = cart.CartItems.Sum(x => x.Quantity),
                TotalPrice = cart.CartItems.Sum(x => x.Quantity * x.UnitPrice),
                FullName = fullName,
                PhoneNumber = phoneNumber,
                City = city,
                District = district,
                Address = address
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            // Form Doğrulaması Başarısısızsa
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var cart = await _cartService.GetCartByUserIdAsync(userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    return RedirectToAction("Index", "Cart");
                }

                model.Cart = cart;
                model.TotalQuantity = cart.CartItems.Sum(x => x.Quantity);
                model.TotalPrice = cart.CartItems.Sum(x => x.Quantity * x.UnitPrice);

                return View(model);
            }

            // Giriş Yapan Kullanıcı Id'sini Al 
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            // Kullanıcın Sepetini Tekrar Getir
            var currentCart = await _cartService.GetCartByUserIdAsync(currentUserId);

            if (currentCart == null || !currentCart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Siparişi Oluştur 
            try
            {
                await _orderService.CreateOrderAsync(currentUserId,
                new CheckoutViewModel
                {
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    City = model.City,
                    District = model.District,
                    Address = model.Address
                },
                currentCart);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                model.Cart = currentCart;
                model.TotalQuantity = currentCart.CartItems.Sum(x => x.Quantity);
                model.TotalPrice = currentCart.CartItems.Sum(x => x.Quantity * x.UnitPrice);

                return View(model);
            }

            // Siparişiniz Başarılıysa Sepeti Temizle
            await _cartService.ClearCartAsync(currentUserId);

            // Başarılı Sayfasına Gönder
            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success() 
        { 
            return View(); 
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            return View(orders);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var order = await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
