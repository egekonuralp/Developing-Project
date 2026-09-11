using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;
using static TechStore.Helpers.ImageValidationHelper;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductImageController : Controller
    {
        private readonly IProductImageService _productImageService;
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;

        public ProductImageController(
            IProductImageService productImageService,
            IProductService productService,
            IWebHostEnvironment environment)
        {
            _productImageService = productImageService;
            _productService = productService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            var images = await _productImageService.GetByProductIdAsync(productId);

            var model = new ProductImageIndexViewModel
            {
                ProductId = productId,
                ProductName = product.Name,
                Images = images
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int productId, IFormFile imageFile)
        {
            var product = await _productImageService.GetByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            if (imageFile == null)
            {
                TempData["Error"] = "Lütfen bir görsel seçiniz.";
                return RedirectToAction(nameof(Index), new { productId });
            }

            if (!IsValidImage(imageFile, out var errorMessage))
            {
                TempData["Error"] = errorMessage;
                return RedirectToAction(nameof(Index), new { productId });
            }

            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                var filePath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products",
                    fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var imageUrl = "/uploads/products/" + fileName;

                await _productImageService.AddImageAsync(productId, imageUrl);

                TempData["Success"] = "Görsel eklendi.";
            }
            catch (InvalidOperationException exception)
            {
                TempData["Error"] = exception.Message;
            }
            catch (Exception)
            {
                TempData["Error"] = "Görsel yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";
            }

            return RedirectToAction(nameof(Index), new { productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            try
            {
                var image = await _productImageService.GetByIdAsync(id);

                if (image != null)
                {
                    var filePath = Path.Combine(
                        _environment.WebRootPath,
                        image.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _productImageService.DeleteImageAsync(id);

                TempData["Success"] = "Görsel silindi.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Görsel silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }

            return RedirectToAction(nameof(Index), new { productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMain(int id, int productId)
        {
            try
            {
                await _productImageService.SetMainImageAsync(id, productId);

                TempData["Success"] = "Ana görsel güncellendi.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ana görsel ayarlanırken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index), new { productId });
        }
    }
}
