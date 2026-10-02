using FoodDelivery.DTOs;
using FoodDelivery.Enums;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Services;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<CouponDto> createCouponAsync(CreateCouponRequest request)
        {
            string codeUpper = request.Code.Trim().ToUpper();

            var existing = await _couponRepository.getByCodeAsync(codeUpper);
            if (existing != null)
                throw new InvalidOperationException($"Coupon code '{codeUpper}' already exists.");

            var coupon = new Coupon
            {
                Code = codeUpper,
                Type = request.Type,
                Value = request.Value,
                MinimumOrderAmount = request.MinimumOrderAmount,
                MaxDiscountAmount = request.MaxDiscountAmount,
                ExpiryDate = request.ExpiryDate,
                IsActive = true,
                UsageLimit = request.UsageLimit,
                TimesUsed = 0
            };

            await _couponRepository.addAsync(coupon);
            await _couponRepository.completeAsync();

            return MapToDto(coupon);
        }

        public async Task<IEnumerable<CouponDto>> getAllActiveCouponsAsync()
        {
            var coupons = await _couponRepository.getActiveCouponsAsync();
            return coupons.Select(MapToDto);
        }

        public async Task<ApplyCouponResponseDto> applyCouponAsync(ApplyCouponRequest request)
        {
            string codeUpper = request.Code.Trim().ToUpper();
            var coupon = await _couponRepository.getByCodeAsync(codeUpper);

            if (coupon == null || !coupon.IsActive)
            {
                return new ApplyCouponResponseDto
                {
                    IsValid = false,
                    Message = "Invalid or inactive coupon code.",
                    Code = codeUpper,
                    DiscountAmount = 0,
                    FinalTotal = request.OrderTotal
                };
            }

            if (coupon.ExpiryDate <= DateTime.UtcNow)
            {
                return new ApplyCouponResponseDto
                {
                    IsValid = false,
                    Message = "Coupon has expired.",
                    Code = codeUpper,
                    DiscountAmount = 0,
                    FinalTotal = request.OrderTotal
                };
            }

            if (coupon.TimesUsed >= coupon.UsageLimit)
            {
                return new ApplyCouponResponseDto
                {
                    IsValid = false,
                    Message = "Coupon usage limit has been reached.",
                    Code = codeUpper,
                    DiscountAmount = 0,
                    FinalTotal = request.OrderTotal
                };
            }

            if (request.OrderTotal < coupon.MinimumOrderAmount)
            {
                return new ApplyCouponResponseDto
                {
                    IsValid = false,
                    Message = $"Minimum order total of ${coupon.MinimumOrderAmount:F2} is required to apply this coupon.",
                    Code = codeUpper,
                    DiscountAmount = 0,
                    FinalTotal = request.OrderTotal
                };
            }

            decimal discount = 0;
            if (coupon.Type == DiscountType.Percentage)
            {
                discount = (request.OrderTotal * coupon.Value) / 100m;
                if (coupon.MaxDiscountAmount > 0 && discount > coupon.MaxDiscountAmount)
                {
                    discount = coupon.MaxDiscountAmount;
                }
            }
            else if (coupon.Type == DiscountType.FlatAmount)
            {
                discount = coupon.Value;
            }

            if (discount > request.OrderTotal)
            {
                discount = request.OrderTotal;
            }

            decimal finalTotal = request.OrderTotal - discount;

            return new ApplyCouponResponseDto
            {
                IsValid = true,
                Message = "Coupon applied successfully!",
                Code = codeUpper,
                DiscountAmount = discount,
                FinalTotal = finalTotal
            };
        }

        public async Task<bool> incrementUsageAsync(string code)
        {
            var coupon = await _couponRepository.getByCodeAsync(code);
            if (coupon == null) return false;

            coupon.TimesUsed++;
            _couponRepository.update(coupon);
            await _couponRepository.completeAsync();
            return true;
        }

        private static CouponDto MapToDto(Coupon coupon)
        {
            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Type = coupon.Type,
                Value = coupon.Value,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                MaxDiscountAmount = coupon.MaxDiscountAmount,
                ExpiryDate = coupon.ExpiryDate,
                IsActive = coupon.IsActive,
                UsageLimit = coupon.UsageLimit,
                TimesUsed = coupon.TimesUsed
            };
        }
    }
}
