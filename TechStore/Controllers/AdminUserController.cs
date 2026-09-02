using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Implementations;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;       
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOrderService _orderService;
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(UserManager<AppUser> userManager, 
            IOrderService orderService, 
            RoleManager<IdentityRole> roleManager,
            IAdminUserService adminUserService)
        {
            _userManager = userManager;
            _orderService = orderService;
            _roleManager = roleManager;
            _adminUserService = adminUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var users = string.IsNullOrWhiteSpace(search)
                ? await _adminUserService.GetAllUserAsync(page, pageSize)
                : await _adminUserService.SearchUserAsync(search, page, pageSize);

            var totalCount = string.IsNullOrWhiteSpace(search)
                ? await _adminUserService.CountAsync()
                : await _adminUserService.SearchCountAsync(search);

            var model = new PagedResultViewModel<AppUser>
            {
                Items = users,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Search = search
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(user.Id);

            var userRoles = await _userManager.GetRolesAsync(user);

            var allRoles = _roleManager.Roles
                .Select(x => x.Name!)
                .ToList();

            var model = new AdminUserDetailViewModel
            {
                User = user,
                OrderCount = orders.Count,
                TotalSpent = orders.Sum(x => x.TotalPrice),
                LastOrderStatus = orders
                    .OrderByDescending(x => x.OrderDate)
                    .Select(x => x.Status)
                    .FirstOrDefault() ?? "-",
                SelectedRole = userRoles.FirstOrDefault() ?? "",
                Roles = allRoles,
                Orders = orders,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(AdminUserRoleUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Detail), new { id = model.UserId });
            }

            var currentUserId = _userManager.GetUserId(User);

            try
            {
                await _adminUserService.UpdateUserRoleAsync(
                    currentUserId!,
                    model.UserId,
                    model.SelectedRole);

                TempData["Success"] = "Rol Başarıyla Güncellendi.";
            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Detail), new { id = model.UserId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == id)
            {
                TempData["Error"] = "Kendi Hesabınızı Silemezsiniz.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _adminUserService.DeleteUserAsync(id);

                TempData["Success"] = "Kullanıcı Başarıyla Silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}