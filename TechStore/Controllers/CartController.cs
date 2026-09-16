using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        private const string AppliedCouponTempDataKey = "AppliedCouponCode";

        public CartController(ICartService cartService, ICouponService couponService)
        {
            _cartService = cartService;
            _couponService = couponService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            try
            {
                await _cartService.AddToCartAsync(userId, productId, quantity);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var viewModel = await _cartService.GetCartSummaryAsync(userId);

            var appliedCode = TempData.Peek(AppliedCouponTempDataKey) as string;

            if (!string.IsNullOrEmpty(appliedCode))
            {
                var coupon = await _couponService.GetByCodeAsync(appliedCode);

                var stillValid = coupon != null &&
                    await _couponService.IsValidAsync(appliedCode, viewModel.TotalPrice);

                if (stillValid)
                {
                    viewModel.AppliedCouponCode = coupon!.Code;
                    viewModel.DiscountAmount = _couponService.CalculateDiscount(coupon, viewModel.TotalPrice);
                }
                else
                {
                    TempData.Remove(AppliedCouponTempDataKey);
                    TempData["Error"] = "Uygulanan kupon artık geçerli değil, sepetinizden kaldırıldı.";
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var cartSummary = await _cartService.GetCartSummaryAsync(userId);

            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["Error"] = "Lütfen bir kupon kodu girin.";
                return RedirectToAction(nameof(Index));
            }

            var isValid = await _couponService.IsValidAsync(code, cartSummary.TotalPrice);

            if (!isValid)
            {
                TempData["Error"] = "Kupon kodu geçersiz, süresi dolmuş ya da sepet tutarınız minimum tutarı karşılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            TempData[AppliedCouponTempDataKey] = code.Trim().ToUpperInvariant();
            TempData["Success"] = "Kupon başarıyla uygulandı.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCoupon()
        {
            TempData.Remove(AppliedCouponTempDataKey);
            TempData["Success"] = "Kupon kaldırıldı.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncreaseQuantity(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            try
            {
                await _cartService.IncreaseQuantityAsync(userId, cartItemId);
            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecreaseQuantity(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            try
            {
                await _cartService.DecreaseQuantityAsync(userId, cartItemId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            try
            {
                await _cartService.RemoveFromCartAsync(userId, cartItemId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}