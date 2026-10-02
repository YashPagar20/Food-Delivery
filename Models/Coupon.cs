using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodDelivery.Enums;

namespace FoodDelivery.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        public DiscountType Type { get; set; } = DiscountType.Percentage;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumOrderAmount { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxDiscountAmount { get; set; } = 0;
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int UsageLimit { get; set; } = 100;
        public int TimesUsed { get; set; } = 0;
    }
}
