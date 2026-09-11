using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.DTOs;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;
using static TechStore.Helpers.ImageValidationHelper;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductImageService _productImageService;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IWebHostEnvironment environment,
            IProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _environment = environment;
            _productImageService = productImageService;
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
               
            if (model.ImageFiles.Count > 8)
            {
                ModelState.AddModelError(
                    nameof(model.ImageFiles),
                    "Bir ürüne en fazla 8 görsel eklenebilir.");

                model.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            foreach (var imageFile in model.ImageFiles)
            {
                if (!IsValidImage(imageFile, out var errorMessage))
                {
                    ModelState.AddModelError(
                        nameof(model.ImageFiles),
                        $"{imageFile.FileName}: {errorMessage}");

                    model.Categories = await _categoryService.GetAllAsync();
                    return View(model);
                }
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                Brand = model.Brand,
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _productService.AddAsync(product);

            foreach (var imageFile in model.ImageFiles)
            {
                var fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                var folderPath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products");

                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var imageUrl = "/uploads/products/" + fileName;

                await _productImageService.AddImageAsync(product.Id, imageUrl);
            }

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

            var galleryImages = await _productImageService.GetByProductIdAsync(id);

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
                GalleryImages = galleryImages,
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

            try
            {
                await _productService.DeleteAsync(product.Id);

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

                TempData["Success"] = "Ürün başarıyla silindi.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Bu ürün kullanıldığı için silinemiyor.";
            }

            return RedirectToAction(nameof(Delete), new { id = product.Id });
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