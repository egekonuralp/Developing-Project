using TechStore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TechStore.ViewModels
{
    public class PaymentViewModel
    {
        [ValidateNever]
        public Cart Cart { get; set; } = null!;

        [ValidateNever]
        public decimal TotalPrice { get; set; }

        [ValidateNever]
        public int TotalQuantity { get; set; }

        [ValidateNever]
        [Required(ErrorMessage = "Ad Soyad Zorunludur.")]
        public string FullName { get; set; } = string.Empty;

        [ValidateNever]
        [Required(ErrorMessage = "Telefon Numarası Zorunludur.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [ValidateNever]
        [Required(ErrorMessage = "Şehir Zorunludur.")]
        public string City { get; set; } = string.Empty;

        [ValidateNever]
        [Required(ErrorMessage = "İlçe Zorunludur.")]
        public string District { get; set; } = string.Empty;

        [ValidateNever]
        [Required(ErrorMessage = "Adres Zorunludur.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kart Numarası Zorunludur.")]
        [StringLength(19, MinimumLength = 16, ErrorMessage = "Kart Numarası 16 - 19 Karakter Arasında Olmalıdır.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Son Kullanma Tarihi Zorunludur.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Son Kullanma Tarihi MM/YY Formatında Olmalıdır.")]
        public string ExpirationDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV >orunludur.")]
        [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "CVV 3 Veya 4 Haneli Olmalıdır.")]
        public string CVV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kart Üzerindeki Ad Soyad Zorunludur.")]
        public string CardHolderName { get; set; } = string.Empty;
    }
}
