using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<Review?> getByOrderIdAsync(int orderId);
        Task<IEnumerable<Review>> getRestaurantReviewsAsync(int restaurantId);
    }
}
