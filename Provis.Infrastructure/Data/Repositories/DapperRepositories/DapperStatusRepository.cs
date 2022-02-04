using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperStatusRepository : IStatusRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperStatusRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "Statuses";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(Status entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name)
                                   VALUES 
                                        (@Name)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<Status> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name)
                                   VALUES 
                                        (@Name)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Status entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Status> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Status>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<Status>(query);
            }
        }

        public async Task<Status> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<Status>(query, new { Id = key[0]});
            }
        }

        public async Task<IEnumerable<Status>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<Status>(sql);
            }
        }

        public async Task UpdateAsync(Status entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET Name = @Name";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<Status> IRepository<Status>.AddAsync(Status entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
