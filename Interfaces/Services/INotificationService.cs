using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationDto> sendNotificationAsync(SendNotificationRequest request);
        Task<IEnumerable<NotificationDto>> getUserNotificationsAsync(int userId);
        Task<bool> markAsReadAsync(int notificationId, int userId);
    }
}
