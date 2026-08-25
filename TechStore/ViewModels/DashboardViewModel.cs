using TechStore.Models;

namespace TechStore.ViewModels
{
    public class DashboardViewModel
    {
        public int ProductCount { get; set; }

        public int CategoryCount { get; set; }

        public int UserCount { get; set; }

        public decimal Revenue { get; set; }

        public int OrderCount { get; set; }

        public List<Product> RecentProducts { get; set; } = new();

        public List<Category> RecentCategories { get; set; } = new();
    }
}
