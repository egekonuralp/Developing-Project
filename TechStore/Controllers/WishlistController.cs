using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechStore.Services.Interfaces;

namespace TechStore.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController (IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlistItems = await _wishlistService.GetByUserIdAsync(userId!);

            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _wishlistService.AddAsync(userId!, productId);

            return Json(new { success = true, isInWishlist = true });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _wishlistService.RemoveAsync(userId!, productId);

            return Json(new { success = true, isInWishlist = false });
        }  
    }
}
