using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon?> getByCodeAsync(string code);
        Task<IEnumerable<Coupon>> getActiveCouponsAsync();
    }
}
