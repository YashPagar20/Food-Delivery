using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment?> getByOrderIdAsync(int orderId);
    }
}
