using TechStore.Data;
using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Services.Implementations
{
    public class SupportService : ISupportService
    {
        private readonly ISupportRepository _supportRepository;

        public SupportService(ISupportRepository supportRepository)
        {
            _supportRepository = supportRepository;
        }

        public async Task CreateTicketAsync(CreateSupportTicketViewModel model, string userId)
        {
            var ticket = new SupportTicket
            {
                UserId = userId,
                Title = model.Title,
                Category = model.Category,
                Status = "Açık",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            var message = new SupportMessage
            {
                SupportTicket = ticket,
                SenderId = userId,
                Message = model.Message,
                IsAdmin = false,
                CreatedDate = DateTime.Now
            };

            await _supportRepository.AddTicketAsync(ticket);
            await _supportRepository.AddMessageAsync(message);
            await _supportRepository.SaveAsync();
        }

        public async Task<List<SupportTicket>> GetUserTicketsAsync(string userId)
        {
            return await _supportRepository.GetUserTicketsAsync(userId);
        }

        public async Task<SupportTicket?> GetTicketByIdAsync(int ticketId)
        {
            return await _supportRepository.GetTicketByIdAsync(ticketId);
        }

        public async Task ReplyToTicketAsync(int ticketId, string message, string senderId, bool isAdmin)
        {
            var ticket = await _supportRepository.GetTicketByIdAsync(ticketId);

            if (ticket == null)
            {
                return;
            }

            if (ticket.Status == "Kapandı")
            {
                return;
            }

            var newMessage = new SupportMessage
            {
                SupportTicketId = ticketId,
                SenderId = senderId,
                Message = message,
                IsAdmin = isAdmin,
                CreatedDate = DateTime.Now
            };

            await _supportRepository.AddMessageAsync(newMessage);
            ticket.UpdatedDate = DateTime.Now;
            await _supportRepository.SaveAsync();
        }

        public async Task<PagedResultViewModel<SupportTicket>> GetPagedTicketsAsync(string? search, string? status, int page, int pageSize)
        {
            var tickets = await _supportRepository.GetPagedTicketsAsync(search, status, page, pageSize);

            var totalCount = await _supportRepository.GetTicketCountAsync(search, status);

            return new PagedResultViewModel<SupportTicket>
            {
                Items = tickets,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Search = search,
            };
        }

        public async Task UpdateTicketStatusAsync(int ticketId, string status)
        {
            var ticket = await _supportRepository.GetTicketByIdAsync(ticketId);

            if (ticket == null)
            {
                return;
            }

            ticket.Status = status;
            ticket.UpdatedDate = DateTime.Now;

            await _supportRepository.SaveAsync();
        }

        public async Task<SupportTicket?> GetTicketByIdAsync(int ticketId, string userId)
        {
            return await _supportRepository.GetTicketByIdAsync(ticketId, userId);
        }
    }
}
