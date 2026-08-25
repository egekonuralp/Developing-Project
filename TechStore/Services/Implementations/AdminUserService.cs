using Microsoft.AspNetCore.Identity;
using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;

namespace TechStore.Services.Implementations
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminUserRepository _adminUserRepository;

        public AdminUserService(UserManager<AppUser> userManager, IAdminUserRepository adminUserRepository)
        {
            _userManager = userManager;
            _adminUserRepository = adminUserRepository;
        }

        public async Task UpdateUserRoleAsync(string currentUserId, string userId, string selectedRole)
        {
            if (currentUserId == userId)
            {
                throw new InvalidOperationException("Kendi Rolünüzü Değiştiremezsiniz.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    throw new Exception("Kullanıcı'nın Mecvut Rolü Kaldırılamadı.");
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, selectedRole);

            if (!addResult.Succeeded)
            {
                throw new Exception("Yeni Rol Atanamadı.");
            }
        }

        public async Task<List<AppUser>> GetAllUserAsync(int page, int pageSize)
        {
            return await _adminUserRepository.GetAllAsync(page, pageSize);
        }

        public async Task<List<AppUser>> SearchUserAsync(string search, int page, int pageSize)
        {
            return await _adminUserRepository.SearchAsync(search, page, pageSize);
        }

        public async Task<int> CountAsync()
        {
            return await _adminUserRepository.CountAsync();
        }

        public async Task<int> SearchCountAsync(string search)
        {
            return await _adminUserRepository.SearchCountAsync(search);
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Kullanıcı Bulunamadı.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

                if (adminUsers.Count <= 1)
                {
                    throw new InvalidOperationException("Sistemde En Az Bir Admin Bulunmalıdır!");
                }
            }

            await _adminUserRepository.DeleteUserAsync(userId);
        }
    }
}
