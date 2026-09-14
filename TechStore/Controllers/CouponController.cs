using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var coupons = await _couponService.GetAllAsync();

            var viewModel = new CouponIndexViewModel
            {
                Coupons = coupons
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CouponCreateViewModel
            {
                DiscountType = DiscountType.Percentage,
                IsActive = true,
                ExpiryDate = DateTime.Now.AddDays(30),
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ExpiryDate <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(model.ExpiryDate),
                    "Son kullanma tarihi gelecekte olmalıdır.");

                return View(model);
            }

            if (model.DiscountType == DiscountType.Percentage &&
                model.DiscountValue > 100)
            {
                ModelState.AddModelError(
                    nameof(model.DiscountValue),
                    "Yüzdesel indirim 100'den büyük olamaz.");

                return View(model);
            }

            if (model.MaxUsageCount.HasValue && 
                model.MaxUsageCount.Value <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.MaxUsageCount),
                    "Kullanım limiti 0'dan büyük olmalıdır.");

                return View(model);
            }

            var coupon = new Coupon
            {
                Code = model.Code.Trim().ToUpperInvariant(),
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                MinOrderAmount = model.MinOrderAmount,
                ExpiryDate = model.ExpiryDate,
                MaxUsageCount = model.MaxUsageCount,
                IsActive = model.IsActive,
            };

            try
            {
                await _couponService.AddAsync(coupon);

                TempData["Success"] = "Kupon başarıyla oluşturuldu.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Kupon oluşturulurken bir hata oluştu.");

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }

            coupon.IsActive = false;

            try
            {
                await _couponService.SetActiveAsync(coupon);

                TempData["Success"] = "Kupon pasifleştirildi.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Kupon aktifleştirilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
