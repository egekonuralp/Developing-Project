using TechStore.Models;

namespace TechStore.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? CouponCode { get; set; } 

        public decimal DiscountAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
