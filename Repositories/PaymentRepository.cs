using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Payment?> getByOrderIdAsync(int orderId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.OrderId == orderId);
        }
    }
}
