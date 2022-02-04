using System;
using System.Collections.Generic;
using Ardalis.Specification;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Provis.Core.Interfaces.Repositories
{
    public interface IReadRepository<TEntity> where TEntity : class, IBaseEntity
    {
        Task<IEnumerable<TEntity>> GetListBySpecAsync(ISpecification<TEntity> specification);
        Task<IEnumerable<TReturn>> GetListBySpecAsync<TReturn>(ISpecification<TEntity, TReturn> specification);
        Task<TEntity> GetFirstBySpecAsync(ISpecification<TEntity> specification);
        Task<bool> AnyBySpecAsync(ISpecification<TEntity> specification);
        Task<bool> AnyBySpecAsync(ISpecification<TEntity> specification, Expression<Func<TEntity, bool>> anyExpression);
        Task<bool> AllBySpecAsync(ISpecification<TEntity> specification, Expression<Func<TEntity, bool>> allExpression);
    }
}
