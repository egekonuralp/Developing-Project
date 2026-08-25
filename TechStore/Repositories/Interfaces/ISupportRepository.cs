using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface ISupportRepository
    {
        Task AddTicketAsync(SupportTicket ticket);

        Task AddMessageAsync(SupportMessage message);

        Task SaveAsync();

        Task<List<SupportTicket>> GetUserTicketsAsync(string userId);

        Task<SupportTicket?> GetTicketByIdAsync(int ticketId);

        Task<List<SupportTicket>> GetPagedTicketsAsync(string? search, string? status, int page, int pageSize);

        Task<int> GetTicketCountAsync(string? search, string? status);

        Task<SupportTicket?> GetTicketByIdAsync(int ticketId, string userId);
    }
}
