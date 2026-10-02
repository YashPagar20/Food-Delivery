using FoodDelivery.Models;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> getByEmailAsync(string email);
        Task<User?> getByUsernameAsync(string username);
    }
}
