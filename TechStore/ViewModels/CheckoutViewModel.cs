using TechStore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TechStore.ViewModels
{
    public class CheckoutViewModel
    {
        [ValidateNever]
        public Cart Cart { get; set; } = null!;

        public int TotalQuantity { get; set; }

        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Ad Soyad Alanı Zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad Soyad Alanı En Fazla 100 Karakter Olabilir.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon Numarası Alanı Zorunludur.")]
        [StringLength(20, ErrorMessage = "Telefon Numarası Alanı En Fazla 15 Karakter Olabilir.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir Alanı Zorunludur.")]
        [StringLength(50, ErrorMessage = "Şehir Alanı En Fazla 50 Karakter Olabilir.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlçe Alanı Zorunludur.")]
        [StringLength(50, ErrorMessage = "İlçe Alanı En Fazla 50 Karakter Olabilir.")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres Alanı Zorunludur.")]
        [StringLength(300, ErrorMessage = "Adres Alanı En Fazla 300 Karakter Olabilir.")]
        public string Address { get; set; } = string.Empty;
    }
}
