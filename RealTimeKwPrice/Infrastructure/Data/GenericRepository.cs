using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly MySqlDatabase _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(MySqlDatabase mySqlDb)
        {
            _context = mySqlDb;
            _dbSet = mySqlDb.Set<T>();
        }
        public async Task<T>? GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
