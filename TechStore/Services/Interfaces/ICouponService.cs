using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface ICouponService
    {
        Task<List<Coupon>> GetAllAsync();

        Task<Coupon?> GetByIdAsync(int id);

        Task<Coupon?> GetByCodeAsync(string code);

        Task AddAsync(Coupon coupon);

        Task<bool> IsValidAsync(string code, decimal orderAmount);

        decimal CalculateDiscount(Coupon coupon, decimal orderAmount);

        Task IncrementUsageAsync(Coupon coupon);

        Task SetActiveAsync(Coupon coupon);
    }
}
