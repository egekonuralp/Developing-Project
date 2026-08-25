using System.ComponentModel.DataAnnotations;

namespace TechStore.ViewModels
{
    public class CreateSupportTicketViewModel
    {
        [Required(ErrorMessage = "Başlık Zorunludur.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Başlık 5 ile 100 karakter arasında olmalıdır.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori Seçiniz.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj Zorunludur.")]
        [MinLength(10, ErrorMessage = "Mesaj en az 10 karakter olmalıdır.")]
        public string Message { get; set; } = string.Empty;
    }
}
