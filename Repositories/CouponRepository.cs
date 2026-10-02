using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Coupon?> getByCodeAsync(string code)
        {
            string codeUpper = code.Trim().ToUpper();
            return await _dbSet.FirstOrDefaultAsync(c => c.Code == codeUpper);
        }

        public async Task<IEnumerable<Coupon>> getActiveCouponsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(c => c.IsActive && c.ExpiryDate > now && c.TimesUsed < c.UsageLimit)
                .ToListAsync();
        }
    }
}
