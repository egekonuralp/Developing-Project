using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Interfaces;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService; 

        public AdminOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(string? search, string? status, int page = 1)
        {
            const int pageSize = 10;
            page = Math.Max(1, page);

            var model = await _orderService.GetPagedOrderAsync(search, status, page, pageSize);

            ViewBag.Status = status;

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var updated = await _orderService.UpdateOrderStatusAsync(id, status);

                TempData[updated ? "Success" : "Error"] = updated
                    ? "Sipariş durumu güncellendi."
                    : "Sipariş bulunamadı.";
            }
            catch (ArgumentException exception)
            {
                TempData["Error"] = exception.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
