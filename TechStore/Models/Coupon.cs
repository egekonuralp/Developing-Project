namespace TechStore.Models
{
    public enum DiscountType
    {
        Percentage = 1,
        FixedAmount = 2
    }

    public class Coupon
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public DiscountType DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public decimal MinOrderAmount { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int? MaxUsageCount { get; set; }

        public int UsageCount { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
