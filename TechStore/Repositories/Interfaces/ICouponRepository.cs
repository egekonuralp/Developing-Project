using TechStore.Models;

namespace TechStore.Repositories.Interfaces
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetByCodeAsync(string code);

        Task<Coupon?> GetByIdAsync(int id);

        Task<List<Coupon>> GetAllAsync();

        Task AddAsync(Coupon coupon);

        void Update(Coupon coupon);

        Task SaveAsync();
    }
}
