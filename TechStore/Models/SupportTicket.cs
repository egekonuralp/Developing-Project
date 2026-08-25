using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TechStore.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public AppUser User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Açık";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
    }
}
