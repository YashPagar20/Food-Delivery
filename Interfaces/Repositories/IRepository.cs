using System.Linq.Expressions;

namespace FoodDelivery.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> getByIdAsync(int id);
        Task<IEnumerable<T>> getAllAsync();
        Task<IEnumerable<T>> findAsync(Expression<Func<T, bool>> predicate);
        Task addAsync(T entity);
        void update(T entity);
        void remove(T entity);
        Task<int> saveChangesAsync();
        Task<int> completeAsync();
    }
}
