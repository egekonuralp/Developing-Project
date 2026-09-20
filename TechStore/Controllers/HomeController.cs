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
        private readonly IWishlistService _wishlistService;
        private readonly IProductImageService _productImageService;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IProductService productService,
            ICategoryService categoryService,
            IReviewService reviewService,
            IWishlistService wishlistService,
            IProductImageService productImageService,
            UserManager<AppUser> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _wishlistService = wishlistService;
            _productImageService = productImageService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId, int Page = 1, int PageSize = 12)
        {
            var filter = new ProductFilterDto
            {
                Page = Page,
                PageSize = PageSize,
                Search = search,
                CategoryId = categoryId,
                IsActive = true
            };

            var totalCount = await _productService.CountAsync(filter);

            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            var products = await _productService.GetActiveProductsAsync(filter);
            var categories = await _categoryService.GetAllAsync();

            var wishlistProductIds = new HashSet<int>();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var wishlistItems = await _wishlistService.GetByUserIdAsync(userId);
                    wishlistProductIds = wishlistItems.Select(x => x.ProductId).ToHashSet();
                }
            }

            var viewModel = new HomeIndexViewModel
            {
                Products = products,
                Categories = categories,
                Search = search,
                SelectedCategoryId = categoryId,
                WishlistProductIds = wishlistProductIds,
                CurrentPage = filter.Page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PageSize = filter.PageSize
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
            var galleryImages = await _productImageService.GetByProductIdAsync(id);
            var relatedProducts = await _productService.GetRelatedProductsAsync(product.CategoryId, product.Id, 4);

            var canReview = false;
            var isInWishlist = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    canReview = !await _reviewService.HasUserReviewedAsync(id, userId);
                    isInWishlist = await _wishlistService.IsInWishlistAsync(userId, id);
                }
            }

            var viewModel = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Reviews = reviews,
                AverageRating = reviews.Any()
                    ? reviews.Average(r => r.Rating)
                    : 0,

                ReviewCount = reviews.Count,
                CanReview = canReview,
                IsInWishlist = isInWishlist,
                GalleryImages = galleryImages,
                RelatedProducts = relatedProducts,
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

                TempData["ReviewMessage"] = "Değerlendirmeniz Başarıyla Eklendi.";
            }
            catch (ArgumentException)
            {
                TempData["ReviewError"] = "Değerlendirmeniz Eklenirken Bir Hata Oluştu.";
            }
            catch (InvalidOperationException)
            {
                TempData["ReviewError"] = "Değerlendirmeniz Eklenirken Bir Hata Oluştu.";
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