using TechStore.DTOs;
using TechStore.Models;

namespace TechStore.ViewModels
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public ProductFilterDto Filter { get; set; } = new();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}