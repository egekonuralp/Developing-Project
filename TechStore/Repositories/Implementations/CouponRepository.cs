using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Repositories.Interfaces;

namespace TechStore.Repositories.Implementations
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public CouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<Coupon?> GetByIdAsync(int id)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task AddAsync(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
        }

        public void Update(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
