using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity: class, IBaseEntity
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByKeyAsync<TKey>(params TKey[] key);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    }
}
