using TechStore.Models;

namespace TechStore.ViewModels
{
    public class AdminOrderDetailViewModel
    {
        //Sipariş Bilgileri
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        //Müşteri Bilgileri
        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        // Ürün Listesi
        public List<OrderItem> Items { get; set; } = new();

        public List<string> StatusOptions { get; set; } = new();

    }
}
