using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.DTOs;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IWebHostEnvironment environment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _environment = environment;
        }

        private static bool IsValidImage(IFormFile file, out string errorMessage)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(extension))
            {
                errorMessage = "Sadece .jpg, .jpeg, .png veya .webp uzantılı dosyalar yüklenebilir.";
                return false;
            }

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedContentTypes.Contains(file.ContentType))
            {
                errorMessage = "Geçersiz dosya türü.";
                return false;
            }

            if (file.Length > MaxImageSizeBytes)
            {
                errorMessage = "Dosya boyutu 5 MB'ı geçemez.";
                return false;
            }

            if (file.Length == 0)
            {
                errorMessage = "Boş dosya yüklenemez.";
                return false;
            }

            if (!HasValidImageSignature(file, extension))
            {
                errorMessage = "Dosya içeriği seçilen görsel türüyle eşleşmiyor.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool HasValidImageSignature(IFormFile file, string extension)
        {
            Span<byte> header = stackalloc byte[12];

            using var stream = file.OpenReadStream();
            var bytesRead = stream.Read(header);

            return extension switch
            {
                ".jpg" or ".jpeg" => bytesRead >= 3 &&
                    header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                ".png" => bytesRead >= 8 &&
                    header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                ".webp" => bytesRead >= 12 &&
                    header[..4].SequenceEqual("RIFF"u8) &&
                    header.Slice(8, 4).SequenceEqual("WEBP"u8),
                _ => false
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            page = Math.Max(1, page);

            var filter = new ProductFilterDto
            {
                Search = search,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Page = page
            };

            var totalCount = await _productService.CountAsync(filter);

            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            var products = await _productService.GetAllAsync(filter);

            var categories = await _categoryService.GetAllAsync();

            var viewModel = new ProductIndexViewModel
            {
                Products = products,
                Categories = categories,
                Filter = filter,

                CurrentPage = filter.Page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PageSize = filter.PageSize
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductCreateViewModel
            {
                Categories = await _categoryService.GetAllAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            string imageUrl = string.Empty;

            if (model.ImageFile != null)
            {
                if (!IsValidImage(model.ImageFile, out var errorMessage))
                {
                    ModelState.AddModelError(nameof(model.ImageFile), errorMessage);
                    model.Categories = await _categoryService.GetAllAsync();
                    return View(model);
                }

                var fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.ImageFile.FileName);

                var folderPath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products");

                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imageUrl = "/uploads/products/" + fileName;
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                Brand = model.Brand,
                CategoryId = model.CategoryId,

                ImageUrl = imageUrl,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _productService.AddAsync(product);

            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                Categories = await _categoryService.GetAllAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            var product = await _productService.GetByIdAsync(model.Id);

            if (product == null)
            {
                return NotFound();
            }

            if (model.ImageFile != null)
            {
                if (!IsValidImage(model.ImageFile, out var errorMessage))
                {
                    ModelState.AddModelError(nameof(model.ImageFile), errorMessage);
                    model.Categories = await _categoryService.GetAllAsync();
                    return View(model);
                }

                var fileName = Guid.NewGuid().ToString() +
                               Path.GetExtension(model.ImageFile.FileName);

                var folderPath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products");

                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldFilePath = Path.Combine(
                        _environment.WebRootPath,
                        product.ImageUrl.TrimStart('/')
                            .Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                product.ImageUrl = "/uploads/products/" + fileName;
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Brand = model.Brand;
            product.CategoryId = model.CategoryId;

            await _productService.UpdateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDeleteViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductDeleteViewModel model)
        {
            var product = await _productService.GetByIdAsync(model.Id);

            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var filePath = Path.Combine(
                    _environment.WebRootPath,
                    product.ImageUrl.TrimStart('/')
                        .Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _productService.DeleteAsync(product.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            await _productService.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _productService.DeactivateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}