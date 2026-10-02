using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> processPaymentAsync(ProcessPaymentRequest request, int customerId);
        Task<PaymentResponseDto?> getPaymentByOrderIdAsync(int orderId, int userId);
    }
}
