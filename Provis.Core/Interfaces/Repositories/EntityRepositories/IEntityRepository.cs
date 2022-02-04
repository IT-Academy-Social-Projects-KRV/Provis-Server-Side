using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Provis.Core.Interfaces.Repositories
{
    public interface IEntityRepository<TEntity> : IRepository<TEntity>, IReadRepository<TEntity>, ISaveChanges where TEntity : class, IBaseEntity
    {
        IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes);
        Task<IDbContextTransaction> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted);
    }
}
