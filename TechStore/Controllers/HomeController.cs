using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechStore.DTOs;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
