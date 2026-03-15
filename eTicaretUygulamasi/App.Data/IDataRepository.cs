using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<T>> GetAll<T>() where T : class;

        // Parametre olarak "params" kullanarak birden fazla tabloyu Include edebiliriz
        Task<T?> GetByIdWithIncludes<T>(int id, params Expression<Func<T, object>>[] includes) where T : class;

        Task<T> Add<T>(T entity) where T : class;
        Task<T> Update<T>(T entity) where T : class;
        Task<T> Delete<T>(T entity) where T : class;
        Task<IEnumerable<T>> GetWhere<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task DeleteRange<T>(IEnumerable<T> entities) where T : class;
        Task<IEnumerable<T>> GetWhereWithIncludes<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class;
        Task<int> Count<T>(Expression<Func<T, bool>> predicate = null) where T : class;
    }
    public class DataRepository : IDataRepository
    {
        private readonly AppDbContext _dbContext;

        public DataRepository(AppDbContext dbContext)
        {

            _dbContext = dbContext;

        }
        public async Task<IEnumerable<T>> GetWhereWithIncludes<T>(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes) where T : class
        {
           
            IQueryable<T> query = _dbContext.Set<T>();

          
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

           
            return await query.Where(predicate).ToListAsync();
        }
        public async Task<int> Count<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate != null)
                return await _dbContext.Set<T>().CountAsync(predicate);

            return await _dbContext.Set<T>().CountAsync();
        }
        public async Task<IEnumerable<T>> GetWhere<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task DeleteRange<T>(IEnumerable<T> entities) where T : class
        {

            if (entities != null && entities.Any())
            {
                _dbContext.Set<T>().RemoveRange(entities);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<T>> GetAll<T>() where T : class
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdWithIncludes<T>(int id, params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            // Gelen tüm Include ifadelerini sorguya ekle
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // EF Core'da her zaman PK üzerinden filtreleme yap (ID ismi farklı olabilir diye Property bazlı gitmek gerekebilir)
            // Ancak en basit haliyle FirstOrDefaultAsync kullanılır:
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public async Task<T> Add<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task<T> Update<T>(T entity) where T : class
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Delete<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
