using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data;
using FoodDelivery.Interfaces.Repositories;

namespace FoodDelivery.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> getByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> getAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> findAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task addAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<int> saveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<int> completeAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
