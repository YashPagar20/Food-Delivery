using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Repositories
{
    public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Restaurant?> getWithMenuItemsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.MenuItems)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Restaurant>> getAllWithMenuItemsAsync()
        {
            return await _dbSet
                .Include(r => r.MenuItems)
                .ToListAsync();
        }
    }
}
