using TechStore.Models;

namespace TechStore.ViewModels
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public int TotalQuantity { get; set; }
    }
}
