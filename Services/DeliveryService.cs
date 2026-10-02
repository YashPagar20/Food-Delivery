using Microsoft.AspNetCore.SignalR;
using FoodDelivery.DTOs;
using FoodDelivery.Enums;
using FoodDelivery.Models;
using FoodDelivery.Hubs;
using FoodDelivery.Interfaces.Services;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly INotificationService _notificationService;

        public DeliveryService(
            IDeliveryRepository deliveryRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IHubContext<OrderHub> hubContext,
            INotificationService notificationService)
        {
            _deliveryRepository = deliveryRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        public async Task<DeliveryDto> assignDriverAsync(AssignDriverRequest request)
        {
            var order = await _orderRepository.getByIdAsync(request.OrderId);
            if (order == null)
                throw new KeyNotFoundException($"Order #{request.OrderId} not found.");

            var driver = await _userRepository.getByIdAsync(request.DriverId);
            if (driver == null || (driver.Role != UserRole.DeliveryPartner && driver.Role != UserRole.Admin))
                throw new InvalidOperationException("Selected user is not a valid delivery partner.");

            var existingDelivery = await _deliveryRepository.getByOrderIdAsync(request.OrderId);
            if (existingDelivery != null)
                throw new InvalidOperationException("Delivery has already been assigned for this order.");

            var delivery = new Delivery
            {
                OrderId = request.OrderId,
                DriverId = request.DriverId,
                DeliveryAddress = request.DeliveryAddress,
                CurrentLocation = "Restaurant Location",
                Status = DeliveryStatus.Assigned,
                AssignedAt = DateTime.UtcNow
            };

            order.Status = OrderStatus.OutForDelivery;
            _orderRepository.update(order);
            await _deliveryRepository.addAsync(delivery);
            await _deliveryRepository.completeAsync();

            await _notificationService.sendNotificationAsync(new SendNotificationRequest
            {
                UserId = request.DriverId,
                Title = "New Delivery Assigned",
                Message = $"You have been assigned to deliver Order #{order.Id} to {request.DeliveryAddress}.",
                Type = NotificationType.InApp
            });

            await _notificationService.sendNotificationAsync(new SendNotificationRequest
            {
                UserId = order.CustomerId,
                Title = "Driver Assigned",
                Message = $"Driver {driver.Username} has been assigned to your Order #{order.Id}.",
                Type = NotificationType.InApp
            });

            await _hubContext.Clients.Group($"Order_{order.Id}")
                .SendAsync("ReceiveDeliveryUpdate", new { OrderId = order.Id, Status = delivery.Status.ToString(), DriverName = driver.Username });

            return MapToDto(delivery, driver.Username);
        }

        public async Task<DeliveryDto?> getDeliveryByOrderIdAsync(int orderId, int userId)
        {
            var delivery = await _deliveryRepository.getByOrderIdAsync(orderId);
            if (delivery == null) return null;

            return MapToDto(delivery, delivery.Driver?.Username ?? "Driver");
        }

        public async Task<IEnumerable<DeliveryDto>> getDriverDeliveriesAsync(int driverId)
        {
            var deliveries = await _deliveryRepository.getByDriverIdAsync(driverId);
            return deliveries.Select(d => MapToDto(d, d.Driver?.Username ?? "Driver"));
        }

        public async Task<bool> updateDeliveryStatusAsync(int deliveryId, UpdateDeliveryStatusRequest request, int driverId)
        {
            var delivery = await _deliveryRepository.getByOrderIdAsync(deliveryId);
            if (delivery == null)
            {
                delivery = await _deliveryRepository.getByIdAsync(deliveryId);
            }

            if (delivery == null || delivery.DriverId != driverId)
                return false;

            delivery.Status = request.Status;

            if (request.Status == DeliveryStatus.PickedUp)
                delivery.PickedUpAt = DateTime.UtcNow;
            else if (request.Status == DeliveryStatus.Delivered)
            {
                delivery.DeliveredAt = DateTime.UtcNow;
                var order = await _orderRepository.getByIdAsync(delivery.OrderId);
                if (order != null)
                {
                    order.Status = OrderStatus.Delivered;
                    _orderRepository.update(order);
                }
            }

            _deliveryRepository.update(delivery);
            await _deliveryRepository.completeAsync();

            await _hubContext.Clients.Group($"Order_{delivery.OrderId}")
                .SendAsync("ReceiveDeliveryUpdate", new { OrderId = delivery.OrderId, Status = delivery.Status.ToString() });

            var relatedOrder = await _orderRepository.getByIdAsync(delivery.OrderId);
            if (relatedOrder != null)
            {
                await _notificationService.sendNotificationAsync(new SendNotificationRequest
                {
                    UserId = relatedOrder.CustomerId,
                    Title = "Delivery Update",
                    Message = $"Your order #{delivery.OrderId} is now {delivery.Status}.",
                    Type = NotificationType.InApp
                });
            }

            return true;
        }

        public async Task<bool> updateDriverLocationAsync(int deliveryId, UpdateLocationRequest request, int driverId)
        {
            var delivery = await _deliveryRepository.getByIdAsync(deliveryId);
            if (delivery == null || delivery.DriverId != driverId)
                return false;

            delivery.CurrentLocation = request.Location;
            _deliveryRepository.update(delivery);
            await _deliveryRepository.completeAsync();

            await _hubContext.Clients.Group($"Order_{delivery.OrderId}")
                .SendAsync("ReceiveDriverLocation", new { OrderId = delivery.OrderId, Location = request.Location });

            return true;
        }

        private static DeliveryDto MapToDto(Delivery delivery, string driverName)
        {
            return new DeliveryDto
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                DriverId = delivery.DriverId,
                DriverName = driverName,
                DeliveryAddress = delivery.DeliveryAddress,
                CurrentLocation = delivery.CurrentLocation,
                Status = delivery.Status,
                AssignedAt = delivery.AssignedAt,
                PickedUpAt = delivery.PickedUpAt,
                DeliveredAt = delivery.DeliveredAt
            };
        }
    }
}
