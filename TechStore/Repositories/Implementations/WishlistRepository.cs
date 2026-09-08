using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Repositories.Interfaces;

namespace TechStore.Repositories.Implementations
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WishlistItem>> GetByUserIdAsync(string userId)
        {
            return await _context.WishlistItems
                .Include(x => x.Product)
                .ThenInclude(x => x.Category)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string userId, int productId)
        {
            return await _context.WishlistItems
                .AnyAsync(x => 
                    x.UserId == userId &&
                    x.ProductId == productId);
        }

        public async Task<WishlistItem?> GetAsync(string userId, int productId)
        {
            return await _context.WishlistItems
                .FirstOrDefaultAsync(x => 
                    x.UserId == userId &&
                    x.ProductId == productId);
        }

        public async Task AddAsync(WishlistItem item)
        {
            await _context.WishlistItems.AddAsync(item);
        }

        public void Delete(WishlistItem item) 
        { 
            _context.WishlistItems.Remove(item); 
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
