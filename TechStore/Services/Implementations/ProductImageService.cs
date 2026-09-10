using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;

namespace TechStore.Services.Implementations
{
    public class ProductImageService : IProductImageService
    {
        private const int MaxImagesPerProduct = 8;
        private readonly IProductImageRepository _productImageRepository;

        public ProductImageService(IProductImageRepository productImageRepository)
        {
            _productImageRepository = productImageRepository;
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _productImageRepository.GetByProductIdAsync(productId);
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            return await _productImageRepository.GetByIdAsync(id);
        }

        public async Task AddImageAsync(int productId, string imageUrl)
        {
            var existingImages = await _productImageRepository.GetByProductIdAsync(productId);

            if (existingImages.Count >= MaxImagesPerProduct)
            {
                throw new InvalidOperationException($"Bir ürüne en fazla {MaxImagesPerProduct} görsel eklenebilir.");
            }

            var image = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                IsMain = existingImages.Count == 0,
                CreatedDate = DateTime.Now,
            };

            await _productImageRepository.AddAsync(image);
            await _productImageRepository.SaveAsync();
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _productImageRepository.GetByIdAsync(imageId);

            if (image == null)
            {
                throw new Exception("Görsel Bulunamadı!");
            }

            var wasMain = image.IsMain;
            var productId = image.ProductId;

            _productImageRepository.Delete(image);
            await _productImageRepository.SaveAsync();

            if (wasMain)
            {
                var remainingImages = await _productImageRepository.GetByProductIdAsync(productId);
                var newMain = remainingImages.FirstOrDefault();

                if (newMain != null)
                {
                    newMain.IsMain = true;
                    await _productImageRepository.SaveAsync();
                }
            }
        }

        public async Task SetMainImageAsync(int productId, int  imageId)
        {
            var images = await _productImageRepository.GetByProductIdAsync(productId);
            var target = images.FirstOrDefault(x => x.Id == imageId);

            if (target == null)
            {
                throw new Exception("Görsel bu ürüne ait değil.");
            }

            foreach (var image in images)
            {
                image.IsMain = image.Id == imageId;
            }

            await _productImageRepository.SaveAsync();
        }
    }
}
