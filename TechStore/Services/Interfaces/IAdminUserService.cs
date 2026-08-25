using Microsoft.AspNetCore.Identity;
using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Services.Interfaces
{
    public interface IAdminUserService
    {
        Task<List<AppUser>> GetAllUserAsync(int page, int pageSize);

        Task<List<AppUser>> SearchUserAsync(string search, int page, int pageSize);

        Task<int> CountAsync();

        Task<int> SearchCountAsync(string search);

        Task UpdateUserRoleAsync(string currentUserId, string userId, string selectedRole);

        Task DeleteUserAsync(string userId);
    }
}