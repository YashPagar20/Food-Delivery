using FoodDelivery.DTOs;
using FoodDelivery.Enums;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Services;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<NotificationDto> sendNotificationAsync(SendNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                Status = NotificationStatus.Sent,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("[NOTIFICATION DISPATCH - {Type}] User #{UserId}: {Title} - {Message}",
                request.Type, request.UserId, request.Title, request.Message);

            await _notificationRepository.addAsync(notification);
            await _notificationRepository.completeAsync();

            return MapToDto(notification);
        }

        public async Task<IEnumerable<NotificationDto>> getUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.getUserNotificationsAsync(userId);
            return notifications.Select(MapToDto);
        }

        public async Task<bool> markAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.getByIdAsync(notificationId);
            if (notification == null || notification.UserId != userId)
                return false;

            notification.IsRead = true;
            _notificationRepository.update(notification);
            await _notificationRepository.completeAsync();
            return true;
        }

        private static NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Status = notification.Status,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
