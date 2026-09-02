namespace TechStore.Models
{
    public static class OrderStatuses
    {
        public const string Preparing = "Hazırlanıyor";
        public const string Shipped = "Kargoya Verildi";
        public const string Delivered = "Teslim Edildi";
        public const string Cancelled = "İptal Edildi";

        private static readonly HashSet<string> ValidStatuses =
            [Preparing, Shipped, Delivered, Cancelled];

        public static bool IsValid(string status) => ValidStatuses.Contains(status);
    }
}
