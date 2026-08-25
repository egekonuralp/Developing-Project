using System.ComponentModel.DataAnnotations;

namespace TechStore.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olmalıdır.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad Zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olmalıdır.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı Adı Zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı Adı en fazla 50 karakter olmalıdır.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-Posta Zorunludur.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon Numarası Zorunludur.")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre Zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre Tekrarı Zorunludur.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
