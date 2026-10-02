using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface IRestaurantRepository : IRepository<Restaurant>
    {
        Task<Restaurant?> getWithMenuItemsAsync(int id);
        Task<IEnumerable<Restaurant>> getAllWithMenuItemsAsync();
    }
}
