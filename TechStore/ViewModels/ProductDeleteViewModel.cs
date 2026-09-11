using TechStore.Models;

namespace TechStore.ViewModels
{
    public class ProductDeleteViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public List<ProductImage> GalleryImages { get; set; } = new();
    }
}