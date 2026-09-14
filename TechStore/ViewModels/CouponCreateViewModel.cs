using System.ComponentModel.DataAnnotations;
using TechStore.Models;

namespace TechStore.ViewModels
{
    public class CouponCreateViewModel
    {
        [Required(ErrorMessage = "Kupon kodu zorunludur.")]
        [StringLength(50, ErrorMessage = "Kupon kodu en fazla 50 karakter olabilir.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "İndirim tipi seçilmelidir.")]
        public DiscountType DiscountType { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "İndirim değeri 0'dan büyük olmalıdır.")]
        public decimal DiscountValue { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Minimum sepet tutarı geçersiz.")]
        public decimal MinOrderAmount { get; set; }

        [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
        public DateTime ExpiryDate { get; set; }

        public int? MaxUsageCount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
