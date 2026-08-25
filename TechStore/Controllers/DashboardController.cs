using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(IProductService productService,
               ICategoryService categoryService, IOrderService orderService,
               UserManager<AppUser> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                ProductCount = await _productService.CountAsync(),

                CategoryCount = await _categoryService.CountAsync(),

                UserCount = _userManager.Users.Count(),

                Revenue = await _orderService.GetTotalRevenueAsync(),

                OrderCount = await _orderService.GetOrderCountAsync(),

                RecentProducts = await _productService.GetRecentProductsAsync(5),

                RecentCategories = await _categoryService.GetRecentCategoriesAsync(5),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDistribution()
        {
            var data = await _productService.GetProductCountByCategoryAsync();

            return Json(data);
        }
    }
}