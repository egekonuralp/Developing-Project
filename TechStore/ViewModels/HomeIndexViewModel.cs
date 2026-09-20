using TechStore.Models;

namespace TechStore.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Product> Products { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public string? Search { get; set; }

        public int? SelectedCategoryId { get; set; }

        public HashSet<int> WishlistProductIds { get; set; } = new();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
