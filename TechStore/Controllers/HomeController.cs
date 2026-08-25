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

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var filter = new ProductFilterDto
            {
                Page = 1,
                PageSize = 12,
                Search = search
            };

            var products = await _productService.GetActiveProductsAsync(filter);

            var viewModel = new HomeIndexViewModel
            {
                Products = products
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
