using System.ComponentModel.DataAnnotations;
using FoodDelivery.Enums;

namespace FoodDelivery.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string? CouponCode { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PlaceOrderRequest
    {
        [Required]
        public int RestaurantId { get; set; }
        [Required, MinLength(1)]
        public List<OrderItemRequest> Items { get; set; } = new();
        public string? CouponCode { get; set; }
    }

    public class OrderItemRequest
    {
        [Required]
        public int MenuItemId { get; set; }
        [Required, Range(1, 100)]
        public int Quantity { get; set; }
    }

    public class UpdateStatusRequest
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
