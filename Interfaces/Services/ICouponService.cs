using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface ICouponService
    {
        Task<CouponDto> createCouponAsync(CreateCouponRequest request);
        Task<IEnumerable<CouponDto>> getAllActiveCouponsAsync();
        Task<ApplyCouponResponseDto> applyCouponAsync(ApplyCouponRequest request);
        Task<bool> incrementUsageAsync(string code);
    }
}
