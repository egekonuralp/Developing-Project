using TechStore.Models;
using TechStore.Repositories.Interfaces;
using TechStore.Services.Interfaces;

namespace TechStore.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _couponRepository.GetAllAsync();
        }

        public async Task<Coupon?> GetByIdAsync(int id)
        {
            return await _couponRepository.GetByIdAsync(id);
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _couponRepository.GetByCodeAsync(code.Trim().ToUpperInvariant());
        }

        public async Task AddAsync(Coupon coupon)
        {
            coupon.Code = coupon.Code.Trim().ToUpperInvariant();
            coupon.CreatedDate = DateTime.Now;
            coupon.UsageCount = 0;

            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveAsync();
        }

        public async Task<bool> IsValidAsync(string code, decimal orderAmount)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var coupon = await _couponRepository.GetByCodeAsync(
                code.Trim().ToUpperInvariant());

            if (coupon == null)
            {
                return false;
            }

            if (!coupon.IsActive)
            {
                return false;
            }

            if (coupon.ExpiryDate < DateTime.Now)
            {
                return false;
            }

            if (orderAmount < coupon.MinOrderAmount)
            {
                return false;
            }

            if (coupon.MaxUsageCount.HasValue && coupon.UsageCount >= coupon.MaxUsageCount.Value)
            {
                return false;
            }

            return true;
        }

        public decimal CalculateDiscount(Coupon coupon, decimal orderAmount)
        {
            if (orderAmount <= 0)
            {
                return 0;
            }

            decimal discount;

            if (coupon.DiscountType == DiscountType.Percentage)
            {
                discount = orderAmount * coupon.DiscountValue / 100m;
            }
            else
            {
                discount = coupon.DiscountValue;
            }

            return Math.Min(discount, orderAmount);
        }

        public async Task IncrementUsageAsync(Coupon coupon)
        {
            coupon.UsageCount++;

            _couponRepository.Update(coupon);

            await _couponRepository.SaveAsync();
        }

        public async Task SetActiveAsync(Coupon coupon)
        {
            _couponRepository.Update(coupon);
            await _couponRepository.SaveAsync();
        }
    }
}