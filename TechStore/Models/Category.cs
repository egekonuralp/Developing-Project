using System.ComponentModel.DataAnnotations;

namespace TechStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori Adı Zorunludur.")]
        [StringLength(50, ErrorMessage = "Kategori Adı En Fazla 50 Karakter Olabilir.")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}