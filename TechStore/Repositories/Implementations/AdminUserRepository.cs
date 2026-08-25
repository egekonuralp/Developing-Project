using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories.Interfaces;

namespace TechStore.Repositories.Implementations
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminUserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<AppUser>> GetAllAsync(int page, int pageSize)
        {
            return await _userManager.Users
                .OrderBy(x => x.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<AppUser>> SearchAsync(string search, int page, int pageSize)
        {
            search = search.ToLower();

            return await _userManager.Users
                .Where(x => 
                    x.Email!.ToLower().Contains(search) || 
                    x.UserName!.ToLower().Contains(search))
                .OrderBy(x => x.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<int> SearchCountAsync(string search)
        {
            search = search.ToLower();

            return await _userManager.Users
                .Where(x =>
                    x.Email!.ToLower().Contains(search) || 
                    x.UserName!.ToLower().Contains(search))
                .CountAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Kullanıcı Bulunamadı.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Kullanıcı Silinemedi.");
            }
        }
    }
}
