using Microsoft.AspNetCore.Identity;
using TechStore.Models;

namespace TechStore.ViewModels
{
    public class AdminUserDetailViewModel
    {
        public AppUser User { get; set; } = null!;

        public int OrderCount { get; set; }

        public decimal TotalSpent { get; set; }

        public string LastOrderStatus { get; set; } = string.Empty;

        public List<Order> Orders { get; set; } = new();

        public string SelectedRole { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();
    }
}
