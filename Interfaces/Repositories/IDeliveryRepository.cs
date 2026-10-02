using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface IDeliveryRepository : IRepository<Delivery>
    {
        Task<Delivery?> getByOrderIdAsync(int orderId);
        Task<IEnumerable<Delivery>> getByDriverIdAsync(int driverId);
    }
}
