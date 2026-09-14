namespace TechStore.ViewModels
{
    public class MyOrdersViewModel
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = string.Empty;

        public int ItemCount { get; set; }
    }
}
