using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize] 
    public class SupportController : Controller
    {
        private readonly ISupportService _supportService;
        private readonly UserManager<AppUser> _userManager;

        public SupportController(ISupportService supportService, UserManager<AppUser> userManager)
        {
            _supportService = supportService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupportTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            await _supportService.CreateTicketAsync(model, userId);

            TempData["Success"] = "Destek Talebiniz Başarıyla Oluşturuldu.";

            return RedirectToAction(nameof(MyTickets));
        }

        public async Task<IActionResult> MyTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            var tickets = await _supportService.GetUserTicketsAsync(userId);

            return View(tickets);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var ticket = await _supportService.GetTicketByIdAsync(id, userId);

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

            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var messageAdded = await _supportService.ReplyToTicketAsync(ticketId, message, userId, false);

            if (!messageAdded)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), new { id = ticketId });
        }
    }
}
