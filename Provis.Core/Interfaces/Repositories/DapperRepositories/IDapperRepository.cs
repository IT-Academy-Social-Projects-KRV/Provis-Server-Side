namespace Provis.Core.Interfaces.Repositories.DapperRepositories
{
    public interface IDapperRepository<TEntity> : IRepository<TEntity>, IDapperQuery<TEntity> where TEntity : class, IBaseEntity
    {

    }
}
