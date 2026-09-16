using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string DeliveryInformationTempDataKey = "DeliveryInformation";
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly ICouponService _couponService;

        public OrderController(ICartService cartService, IOrderService orderService, ICouponService couponService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _couponService = couponService;
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

            if (cart.CartItems.Any(x => !x.Product.IsActive))
            {
                TempData["Error"] = "Sepetinizde satışta olmayan ürün(ler) var. Siparişi tamamlayabilmek için lütfen bu ürünleri sepetinizden kaldırın.";
                return RedirectToAction("Index", "Cart");
            }

            var totalPrice = cart.CartItems.Sum(x => x.Quantity * x.UnitPrice);

            var appliedCouponCode = TempData.Peek("AppliedCouponCode") as string;

            decimal discountAmount = 0;

            if (!string.IsNullOrWhiteSpace(appliedCouponCode))
            {
                var coupon = await _couponService.GetByCodeAsync(appliedCouponCode);

                if (coupon != null && await _couponService.IsValidAsync(appliedCouponCode, totalPrice))
                {
                    discountAmount = _couponService.CalculateDiscount(coupon, totalPrice);
                }
                else
                {
                    TempData.Remove("AppliedCouponCode");

                    TempData["Error"] = "Uygulanan kupon artık geçerli değil.";

                    appliedCouponCode = null;
                }
            }

            var viewModel = new CheckoutViewModel
            {
                Cart = cart,
                TotalQuantity = cart.CartItems.Sum(x => x.Quantity),
                TotalPrice = totalPrice,
                AppliedCouponCode = appliedCouponCode,
                DiscountAmount = discountAmount
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
                
                var totalPrice = cart!.CartItems.Sum(x => x.Quantity * x.UnitPrice);

                model.TotalQuantity = cart.CartItems.Sum(x => x.Quantity);
                model.TotalPrice = totalPrice;

                var appliedCouponCode = TempData.Peek("AppliedCouponCode") as string;

                if (!string.IsNullOrWhiteSpace(appliedCouponCode))
                {
                    var coupon = await _couponService.GetByCodeAsync(appliedCouponCode);

                    if (coupon != null && await _couponService.IsValidAsync(appliedCouponCode, totalPrice))
                    {
                        model.AppliedCouponCode = appliedCouponCode;
                        model.DiscountAmount = _couponService.CalculateDiscount(coupon, totalPrice);
                    }
                }

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

            if (currentCart.CartItems.Any(x => !x.Product.IsActive))
            {
                TempData["Error"] = "Sepetinizde satışta olmayan ürün(ler) var. Siparişi tamamlayabilmek için lütfen bu ürünleri sepetinizden kaldırın.";
                return RedirectToAction("Index", "Cart");
            }

            var deliveryInformation = new DeliveryInformationViewModel
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                District = model.District,
                Address = model.Address
            };

            TempData[DeliveryInformationTempDataKey] = JsonSerializer.Serialize(deliveryInformation);

            return RedirectToAction(nameof(Payment));
        }

        [HttpGet]
        public async Task<IActionResult> Payment()
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

            if (cart.CartItems.Any(x => !x.Product.IsActive))
            {
                TempData["Error"] = "Sepetinizde satışta olmayan ürün(ler) var. Siparişi tamamlayabilmek için lütfen bu ürünleri sepetinizden kaldırın.";
                return RedirectToAction("Index", "Cart");
            }

            var deliveryInformation = GetDeliveryInformation();

            if (deliveryInformation == null)
            {
                TempData["Error"] = "Ödeme adımına geçmeden önce teslimat bilgilerinizi giriniz.";
                return RedirectToAction(nameof(Checkout));
            }

            var totalPrice = cart.CartItems.Sum(x => x.Quantity * x.UnitPrice);

            var appliedCouponCode = TempData.Peek("AppliedCouponCode") as string;

            decimal discountAmount = 0;

            if (!string.IsNullOrWhiteSpace(appliedCouponCode))
            {
                var coupon = await _couponService.GetByCodeAsync(appliedCouponCode);

                if (coupon != null &&
                    await _couponService.IsValidAsync(appliedCouponCode, totalPrice))
                {
                    discountAmount = _couponService.CalculateDiscount(coupon, totalPrice);
                }
                else
                {
                    TempData.Remove("AppliedCouponCode");

                    TempData["Error"] = "Uygulanan kupon artık geçerli değil.";

                    appliedCouponCode = null;
                }
            }

            var model = new PaymentViewModel
            {
                Cart = cart,
                TotalQuantity = cart.CartItems.Sum(x => x.Quantity),
                TotalPrice = totalPrice,
                DiscountAmount = discountAmount,
                AppliedCouponCode = appliedCouponCode,
                FullName = deliveryInformation.FullName,
                PhoneNumber = deliveryInformation.PhoneNumber,
                City = deliveryInformation.City,
                District = deliveryInformation.District,
                Address = deliveryInformation.Address
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            var deliveryInformation = GetDeliveryInformation();

            if (deliveryInformation == null)
            {
                TempData["Error"] = "Ödeme oturumunuz sona erdi. Lütfen teslimat bilgilerinizi tekrar giriniz.";
                return RedirectToAction(nameof(Checkout));
            }

            var appliedCouponCode = TempData.Peek("AppliedCouponCode") as string;

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

                var totalPrice = cart.CartItems.Sum(x => x.Quantity * x.UnitPrice);

                model.TotalQuantity = cart.CartItems.Sum(x => x.Quantity);
                model.TotalPrice = totalPrice;

                if (!string.IsNullOrWhiteSpace(appliedCouponCode))
                {
                    var coupon = await _couponService.GetByCodeAsync(appliedCouponCode);

                    if (coupon != null &&
                        await _couponService.IsValidAsync(appliedCouponCode, totalPrice))
                    {
                        model.AppliedCouponCode = appliedCouponCode;
                        model.DiscountAmount = _couponService.CalculateDiscount(coupon, totalPrice);
                    }
                }

                ApplyDeliveryInformation(model, deliveryInformation);

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

            try
            {
                await _orderService.CreateOrderAsync(currentUserId,
                new CheckoutViewModel
                {
                    FullName = deliveryInformation.FullName,
                    PhoneNumber = deliveryInformation.PhoneNumber,
                    City = deliveryInformation.City,
                    District = deliveryInformation.District,
                    Address = deliveryInformation.Address
                },
                currentCart,
                appliedCouponCode);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                model.Cart = currentCart;

                var totalPrice = currentCart.CartItems.Sum(x => x.Quantity * x.UnitPrice);

                model.TotalQuantity = currentCart.CartItems.Sum(x => x.Quantity);
                model.TotalPrice = totalPrice;

                if (!string.IsNullOrWhiteSpace(appliedCouponCode))
                {
                    var coupon = await _couponService.GetByCodeAsync(appliedCouponCode);

                    if (coupon != null &&
                        await _couponService.IsValidAsync(appliedCouponCode, totalPrice))
                    {
                        model.AppliedCouponCode = appliedCouponCode;
                        model.DiscountAmount = _couponService.CalculateDiscount(coupon, totalPrice);
                    }
                }

                ApplyDeliveryInformation(model, deliveryInformation);

                return View(model);
            }

            TempData.Remove(DeliveryInformationTempDataKey);

            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success() 
        { 
            return View(); 
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            var viewModel = new MyOrdersListViewModel
            {
                Orders = orders.Select(order => new MyOrdersViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status,
                    ItemCount = order.OrderItems.Count
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderDetailViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                FullName = order.FullName,
                Phone = order.Phone,
                City = order.City,
                District = order.District,
                Address = order.Address,
                CouponCode = order.CouponCode,
                DiscountAmount = order.DiscountAmount,
                OrderItems = order.OrderItems.ToList()
            };

            return View(viewModel);
        }

        private DeliveryInformationViewModel? GetDeliveryInformation()
        {
            var serializedValue = TempData.Peek(DeliveryInformationTempDataKey) as string;

            if (string.IsNullOrWhiteSpace(serializedValue))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<DeliveryInformationViewModel>(serializedValue);
            }
            catch (JsonException)
            {
                TempData.Remove(DeliveryInformationTempDataKey);
                return null;
            }
        }

        private static void ApplyDeliveryInformation(PaymentViewModel payment, DeliveryInformationViewModel delivery)
        {
            payment.FullName = delivery.FullName;
            payment.PhoneNumber = delivery.PhoneNumber;
            payment.City = delivery.City;
            payment.District = delivery.District;
            payment.Address = delivery.Address;
        }
    }
}
