using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> getUserNotificationsAsync(int userId);
    }
}
