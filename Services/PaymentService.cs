using FoodDelivery.DTOs;
using FoodDelivery.Enums;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Services;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<PaymentResponseDto> processPaymentAsync(ProcessPaymentRequest request, int customerId)
        {
            var order = await _orderRepository.getByIdAsync(request.OrderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

            if (order.CustomerId != customerId)
                throw new UnauthorizedAccessException("You are not authorized to process payment for this order.");

            var existingPayment = await _paymentRepository.getByOrderIdAsync(request.OrderId);
            if (existingPayment != null && existingPayment.Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Payment has already been completed for this order.");

            decimal amountToPay = order.FinalPrice > 0 ? order.FinalPrice : order.TotalPrice;
            string txnId = "TXN_" + Guid.NewGuid().ToString("N")[..12].ToUpper();

            Payment payment;
            if (existingPayment != null)
            {
                payment = existingPayment;
                payment.Method = request.Method;
                payment.Amount = amountToPay;
                payment.Status = PaymentStatus.Completed;
                payment.TransactionId = txnId;
                payment.PaidAt = DateTime.UtcNow;
                _paymentRepository.update(payment);
            }
            else
            {
                payment = new Payment
                {
                    OrderId = request.OrderId,
                    Amount = amountToPay,
                    Method = request.Method,
                    Status = PaymentStatus.Completed,
                    TransactionId = txnId,
                    CreatedAt = DateTime.UtcNow,
                    PaidAt = DateTime.UtcNow
                };
                await _paymentRepository.addAsync(payment);
            }

            order.Status = OrderStatus.Preparing;
            _orderRepository.update(order);
            await _paymentRepository.completeAsync();

            await _notificationService.sendNotificationAsync(new SendNotificationRequest
            {
                UserId = customerId,
                Title = "Payment Successful",
                Message = $"Payment of ${amountToPay:F2} for Order #{order.Id} was successful (Txn: {txnId}).",
                Type = NotificationType.InApp
            });

            return MapToDto(payment);
        }

        public async Task<PaymentResponseDto?> getPaymentByOrderIdAsync(int orderId, int userId)
        {
            var order = await _orderRepository.getByIdAsync(orderId);
            if (order == null) return null;

            if (order.CustomerId != userId)
            {
                var user = await _userRepository.getByIdAsync(userId);
                if (user?.Role != UserRole.Admin && user?.Role != UserRole.RestaurantOwner)
                    throw new UnauthorizedAccessException("Not authorized to view payment details.");
            }

            var payment = await _paymentRepository.getByOrderIdAsync(orderId);
            return payment == null ? null : MapToDto(payment);
        }

        private static PaymentResponseDto MapToDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt
            };
        }
    }
}
