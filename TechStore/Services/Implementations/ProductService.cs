using TechStore.DTOs;
using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;

namespace TechStore.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllAsync(ProductFilterDto filter)
        {
            return await _productRepository.GetAllAsync(filter);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new Exception("Ürün adı boş olamaz.");
            }

            if (product.Price <= 0)
            {
                throw new Exception("Ürün fiyatı sıfırdan büyük olmalıdır.");
            }

            if (product.Stock < 0)
            {
                throw new Exception("Stok negatif olamaz.");
            }

            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new Exception("Ürün adı boş olamaz.");
            }

            if (product.Price <= 0)
            {
                throw new Exception("Ürün fiyatı sıfırdan büyük olmalıdır.");
            }

            if (product.Stock < 0)
            {
                throw new Exception("Stok negatif olamaz.");
            }

            _productRepository.Update(product);
            await _productRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception("Ürün bulunamadı.");
            }

            _productRepository.Delete(product);

            await _productRepository.SaveAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception("Ürün bulunamadı.");
            }

            product.IsActive = true;

            await _productRepository.SaveAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception("Ürün bulunamadı.");
            }

            product.IsActive = false;

            await _productRepository.SaveAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _productRepository.CountAsync();
        }

        public async Task<List<Product>> GetRecentProductsAsync(int count)
        {
            return await _productRepository.GetRecentProductsAsync(count);
        }

        public async Task<List<CategoryProductCountDto>> GetProductCountByCategoryAsync()
        {
            return await _productRepository.GetProductCountByCategoryAsync();
        }

        public async Task<int> CountAsync(ProductFilterDto filter)
        {
            return await _productRepository.CountAsync(filter);
        }

        public async Task<List<Product>> GetActiveProductsAsync(ProductFilterDto filter)
        {
            return await _productRepository.GetActiveProductsAsync(filter);
        }
    }
}