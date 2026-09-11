using TechStore.Models;

namespace TechStore.ViewModels
{
    public class ProductImageIndexViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public List<ProductImage> Images { get; set; } = new();
    }
}
