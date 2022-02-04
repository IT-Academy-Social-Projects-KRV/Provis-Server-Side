using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperStatusHistoryRepository : IStatusHistoryRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperStatusHistoryRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "StatusHistories";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(StatusHistory entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (DateOfChange, 
                                         TaskId,
                                         StatusId,
                                         UserId)
                                   VALUES 
                                        (@DateOfChange, 
                                         @TaskId,
                                         @StatusId,
                                         @UserId)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<StatusHistory> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (DateOfChange, 
                                         TaskId,
                                         StatusId,
                                         UserId)
                                   VALUES 
                                        (@DateOfChange, 
                                         @TaskId,
                                         @StatusId,
                                         @UserId)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(StatusHistory entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<StatusHistory> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<StatusHistory>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[DateOfChange]
                                     ,[TaskId]
                                     ,[StatusId]
                                     ,[UserId]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<StatusHistory>(query);
            }
        }

        public async Task<StatusHistory> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[DateOfChange]
                                     ,[TaskId]
                                     ,[StatusId]
                                     ,[UserId]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<StatusHistory>(query, new { Id = key[0] });
            }
        }

        public async Task<IEnumerable<StatusHistory>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<StatusHistory>(sql);
            }
        }

        public async Task UpdateAsync(StatusHistory entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET DateOfChange = @DateOfChange,
                                 StatusId = @StatusId,
                                 TaskId = @TaskId,
                                 UserId = @UserId";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<StatusHistory> IRepository<StatusHistory>.AddAsync(StatusHistory entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
