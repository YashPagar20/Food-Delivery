using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Order?> getOrderWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Coupon)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> getUserOrdersWithDetailsAsync(int customerId)
        {
            return await _dbSet
                .Include(o => o.Coupon)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> getRestaurantOrdersWithDetailsAsync(int restaurantId)
        {
            return await _dbSet
                .Include(o => o.Coupon)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.RestaurantId == restaurantId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
