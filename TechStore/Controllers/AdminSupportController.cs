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
            page = Math.Max(1, page);

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

            var messageAdded = await _supportService.ReplyToTicketAsync(ticketId, message, adminId, true);

            if (!messageAdded)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int ticketId, string status)
        {
            try
            {
                var updated = await _supportService.UpdateTicketStatusAsync(ticketId, status);

                TempData[updated ? "Success" : "Error"] = updated
                    ? "Destek talebi durumu güncellendi."
                    : "Destek talebi bulunamadı.";
            }
            catch (ArgumentException exception)
            {
                TempData["Error"] = exception.Message;
            }

            return RedirectToAction(nameof(Detail), new { id = ticketId });
        }
    }
}
