using System.ComponentModel.DataAnnotations;
using FoodDelivery.Enums;

namespace FoodDelivery.DTOs
{
    public class CreateCouponRequest
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DiscountType Type { get; set; }

        [Required, Range(0.01, 10000.00)]
        public decimal Value { get; set; }

        public decimal MinimumOrderAmount { get; set; } = 0;
        public decimal MaxDiscountAmount { get; set; } = 0;

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int UsageLimit { get; set; } = 100;
    }

    public class ApplyCouponRequest
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required, Range(0.01, 100000.00)]
        public decimal OrderTotal { get; set; }
    }

    public class ApplyCouponResponseDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
    }

    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; }
        public int TimesUsed { get; set; }
    }
}
