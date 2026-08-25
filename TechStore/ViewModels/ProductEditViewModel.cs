using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TechStore.Models;

namespace TechStore.ViewModels
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 999999)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Marka zorunludur.")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçmelisiniz.")]
        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public List<Category> Categories { get; set; } = new();
    }
}