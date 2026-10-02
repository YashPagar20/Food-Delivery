using System.ComponentModel.DataAnnotations;
using FoodDelivery.Enums;

namespace FoodDelivery.DTOs
{
    public class ProcessPaymentRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }
    }

    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
