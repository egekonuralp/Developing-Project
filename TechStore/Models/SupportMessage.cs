using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TechStore.Models
{
    public class SupportMessage
    {
        public int Id { get; set; }

        [Required]
        public int SupportTicketId { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public SupportTicket SupportTicket { get; set; } = null!;

        public AppUser Sender { get; set; } = null!;
    }
}
