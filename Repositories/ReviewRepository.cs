using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Review?> getByOrderIdAsync(int orderId)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        public async Task<IEnumerable<Review>> getRestaurantReviewsAsync(int restaurantId)
        {
            return await _dbSet
                .Include(r => r.Customer)
                .Where(r => r.RestaurantId == restaurantId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
