using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Repositories.Interfaces;

namespace TechStore.Repositories.Implementations
{
    public class SupportRepository : ISupportRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTicketAsync(SupportTicket ticket)
        {
            await _context.SupportTickets.AddAsync(ticket);
        }

        public async Task AddMessageAsync(SupportMessage message)
        {
            await _context.SupportMessages.AddAsync(message);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<SupportTicket>> GetUserTicketsAsync(string userID)
        {
            return await _context.SupportTickets
                .Where(x => x.UserId == userID)
                .OrderByDescending(x => x.UpdatedDate)
                .ToListAsync();
        }

        public async Task<SupportTicket?> GetTicketByIdAsync(int ticketId)
        {
            return await _context.SupportTickets
                .Include(x => x.User)
                .Include(x => x.Messages)
                .ThenInclude(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == ticketId);
        }

        public async Task<List<SupportTicket>> GetPagedTicketsAsync(string? search, string? status, int page, int pageSize)
        {
            var query = _context.SupportTickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(search) ||
                    x.Category.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            return await query
                .OrderByDescending(x => x.UpdatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTicketCountAsync(string? search, string? status)
        {
            var query = _context.SupportTickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(search) ||
                    x.Category.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            return await query.CountAsync();
        }

        public async Task<SupportTicket?> GetTicketByIdAsync(int ticketId, string userId)
        {
            return await _context.SupportTickets
                .Include(x => x.User)
                .Include(x => x.Messages)
                .ThenInclude(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == ticketId && x.UserId == userId);
        }
    }
}
