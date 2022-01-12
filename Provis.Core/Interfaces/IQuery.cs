using System.Linq;

namespace Provis.Core.Interfaces
{
    public interface IQuery<TEntity>
    {
        IQueryable<TEntity> Query { get; }
    }
}
