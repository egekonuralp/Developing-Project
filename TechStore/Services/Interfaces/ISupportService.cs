using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Services.Interfaces
{
    public interface ISupportService
    {
        Task CreateTicketAsync(CreateSupportTicketViewModel model, string userId);

        Task<List<SupportTicket>> GetUserTicketsAsync(string userId);

        Task<SupportTicket?> GetTicketByIdAsync(int ticketId);

        Task ReplyToTicketAsync(int ticketId, string message, string senderId, bool isAdmin);

        Task<PagedResultViewModel<SupportTicket>> GetPagedTicketsAsync(string? search, string? status, int page, int pageSize);

        Task UpdateTicketStatusAsync(int ticketId, string status);

        Task<SupportTicket?> GetTicketByIdAsync(int ticketId, string userId);
    }
}
