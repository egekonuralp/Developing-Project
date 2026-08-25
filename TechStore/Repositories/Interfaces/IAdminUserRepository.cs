using Microsoft.AspNetCore.Identity;
using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface IAdminUserRepository
    {
        Task<List<AppUser>> GetAllAsync(int page, int pageSize);

        Task<List<AppUser>> SearchAsync(string search, int page, int pageSize);

        Task<int> CountAsync();

        Task<int> SearchCountAsync(string search);

        Task DeleteUserAsync(string userId); 
    }
}
