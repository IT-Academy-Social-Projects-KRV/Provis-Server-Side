using Microsoft.EntityFrameworkCore;
using Provis.Core.Interfaces;
using Provis.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories
{
    public class BaseRepository<TEntity>: IRepository<TEntity> where TEntity: class, IBaseEntity
    {
        protected readonly ProvisDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(ProvisDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByKeyAsync<TKey>(TKey key)
        {
            return await _dbSet.FindAsync(key);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return (await _dbSet.AddAsync(entity)).Entity;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => _dbContext.Entry(entity).State = EntityState.Modified);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(()=> _dbSet.Remove(entity));
        }

        public IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = includes
                .Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>(_dbSet, (current, include) => current.Include(include));

            return query;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
