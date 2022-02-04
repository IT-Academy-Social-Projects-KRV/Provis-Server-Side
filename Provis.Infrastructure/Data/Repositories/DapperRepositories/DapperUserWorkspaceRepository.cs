using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories
{
    class DapperUserWorkspaceRepository : IUserWorkspaceRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperUserWorkspaceRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "UserWorkspaces";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<UserWorkspace> AddAsync(UserWorkspace entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (WorkspaceId,
                                         UserId, 
                                         RoleId)
                                   VALUES 
                                        (@WorkspaceId, 
                                         @UserId, 
                                         @RoleId)";

                await db.ExecuteAsync(query, entity);
                return entity;
            }
        }

        public async Task AddRangeAsync(List<UserWorkspace> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (WorkspaceId,
                                         UserId, 
                                         RoleId)
                                   VALUES 
                                        (@WorkspaceId, 
                                         @UserId, 
                                         @RoleId)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(UserWorkspace entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE WorkspaceId = @WorkspaceId,
                                     UserId = @UserId";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<UserWorkspace> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE WorkspaceId = @WorkspaceId,
                                     UserId = @UserId";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<UserWorkspace>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [WorkspaceId]
                                     ,[UserId]
                                     ,[RoleId]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<UserWorkspace>(query);
            }
        }

        public async Task<UserWorkspace> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [WorkspaceId]
                                     ,[UserId]
                                     ,[RoleId]
                            FROM [dbo].[{_tableName}]
                            WHERE WorkspaceId = @WorkspaceId AND
                                  UserId = @UserId";

                return await db.QueryFirstAsync<UserWorkspace>(query, new { WorkspaceId = key[0], UserId = key[1]});
            }
        }

        public async Task<IEnumerable<UserWorkspace>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<UserWorkspace>(sql);
            }
        }

        public async Task UpdateAsync(UserWorkspace entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET WorkspaceId = @WorkspaceId,
                                 UserId = @UserId,
                                 RoleId = @RoleId";

                await db.ExecuteAsync(query, entity);
            }
        }
    }
}
