using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Repositories.DapperRepositories
{
    public interface IDapperQuery<TEntity> where TEntity : class, IBaseEntity
    {
        Task<IEnumerable<TEntity>> Query(string sql);
    }
}
