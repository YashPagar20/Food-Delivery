using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Repositories
{
    public class DeliveryRepository : GenericRepository<Delivery>, IDeliveryRepository
    {
        public DeliveryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Delivery?> getByOrderIdAsync(int orderId)
        {
            return await _dbSet
                .Include(d => d.Driver)
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.OrderId == orderId);
        }

        public async Task<IEnumerable<Delivery>> getByDriverIdAsync(int driverId)
        {
            return await _dbSet
                .Include(d => d.Driver)
                .Where(d => d.DriverId == driverId)
                .OrderByDescending(d => d.AssignedAt)
                .ToListAsync();
        }
    }
}
