using TechStore.Models;

namespace TechStore.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Product> Products { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public string? Search { get; set; }

        public int? SelectedCategoryId { get; set; }
    }
}
