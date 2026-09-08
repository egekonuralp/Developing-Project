using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;
using TechStore.DTOs;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IProductService productService, 
            ICategoryService categoryService, 
            IReviewService reviewService,
            UserManager<AppUser> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId, int Page = 1, int PageSize = 12)
        {
            var filter = new ProductFilterDto
            {
                Page = Page,
                PageSize = PageSize,
                Search = search,
                CategoryId = categoryId
            };

            var products = await _productService.GetActiveProductsAsync(filter);
            var categories = await _categoryService.GetAllAsync();

            var viewModel = new HomeIndexViewModel
            {
                Products = products,
                Categories = categories,
                Search = search,
                SelectedCategoryId = categoryId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null || !product.IsActive)
            {
                return NotFound();
            }

            var reviews = await _reviewService.GetByProductIdAsync(id);

            var canReview = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    canReview = !await _reviewService.HasUserReviewedAsync(id, userId);
                }
            }

            var viewModel = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Reviews = reviews,
                AverageRating = reviews.Any()
                    ? reviews.Average(r => r.Rating)
                    : 0,

                ReviewCount = reviews.Count,
                CanReview = canReview
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                await _reviewService.AddReviewAsync(productId, userId, rating, comment);

                TempData["ReviewMessage"] = "Deðerlendirmeniz Baþarýyla Eklendi.";
            }
            catch (ArgumentException)
            {
                TempData["ReviewError"] = "Deðerlendirmeniz Eklenirken Bir Hata Oluþtu.";
            }
            catch (InvalidOperationException)
            {
                TempData["ReviewError"] = "Deðerlendirmeniz Eklenirken Bir Hata Oluþtu.";
            }

            return RedirectToAction(nameof(Details), new { id = productId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
