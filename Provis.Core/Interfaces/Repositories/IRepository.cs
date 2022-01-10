using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity: class, IBaseEntity
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByKeyAsync<TKey>(TKey key);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes);
        Task<int> SaveChangesAsync();
        Task AddRangeAsync(List<TEntity> entities);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
