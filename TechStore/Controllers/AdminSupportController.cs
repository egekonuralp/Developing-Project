using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Interfaces;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSupportController : Controller
    {
        private readonly ISupportService _supportService;
        private readonly UserManager<AppUser> _userManager;

        public AdminSupportController(ISupportService supportService, UserManager<AppUser> userManager)
        {
            _supportService = supportService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int page = 1)
        {
            const int pageSize = 10;

            var model = await _supportService.GetPagedTicketsAsync(search, status, page, pageSize);
            
            ViewBag.Status = status;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var ticket = await _supportService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMessage(int ticketId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return RedirectToAction(nameof(Detail), new { id = ticketId });
            }

            var adminId = _userManager.GetUserId(User);

            if (adminId == null)
            {
                return Challenge();
            }

            await _supportService.ReplyToTicketAsync(ticketId, message, adminId, true);

            return RedirectToAction(nameof(Detail), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int ticketId, string status)
        {
            await _supportService.UpdateTicketStatusAsync(ticketId, status);

            return RedirectToAction(nameof(Detail), new { id = ticketId });
        }
    }
}