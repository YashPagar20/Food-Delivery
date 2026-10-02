using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> getOrderWithDetailsAsync(int id);
        Task<IEnumerable<Order>> getUserOrdersWithDetailsAsync(int customerId);
        Task<IEnumerable<Order>> getRestaurantOrdersWithDetailsAsync(int restaurantId);
    }
}
