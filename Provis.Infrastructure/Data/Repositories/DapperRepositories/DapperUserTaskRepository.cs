using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperUserTaskRepository : IUserTaskRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperUserTaskRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "UserTask";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(UserTask entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (IsUserDeleted,
                                         TaskId, 
                                         UserId,
                                         UserRoleTagId)
                                   VALUES 
                                        (@IsUserDeleted, 
                                         @UserId, 
                                         @ImageAvatarUrl,
                                         @UserRoleTagId)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<UserTask> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (IsUserDeleted,
                                         TaskId, 
                                         UserId,
                                         UserRoleTagId)
                                   VALUES 
                                        (@IsUserDeleted, 
                                         @UserId, 
                                         @ImageAvatarUrl,
                                         @UserRoleTagId)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(UserTask entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<UserTask> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<UserTask>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [TaskId]
                                     ,[UserId]
                                     ,[IsUserDeleted]
                                     ,[UserRoleTagId]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<UserTask>(query);
            }
        }

        public async Task<UserTask> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [TaskId]
                                     ,[UserId]
                                     ,[IsUserDeleted]
                                     ,[UserRoleTagId]
                            FROM [dbo].[{_tableName}]
                            WHERE TaskId = @TaskId AND
                                  UserId = @UserId";

                return await db.QueryFirstAsync<UserTask>(query, new { TaskId = key[0], UserId = key[1] });
            }
        }

        public async Task<IEnumerable<UserTask>> Query(string sql)
        {
            using(IDbConnection db = Connection)
            {
                return await db.QueryAsync<UserTask>(sql);
            }
        }

        public async Task UpdateAsync(UserTask entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET IsUserDeleted = @IsUserDeleted,
                                 TaskId = @TaskId, 
                                 UserId = @UserId,
                                 UserRoleTagId = @UserRoleTagId";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<UserTask> IRepository<UserTask>.AddAsync(UserTask entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
