using TechStore.Models;

namespace TechStore.ViewModels
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public int TotalQuantity { get; set; }

        public bool HasInactiveItems => Cart.CartItems.Any(x => !x.Product.IsActive);
    }
}
