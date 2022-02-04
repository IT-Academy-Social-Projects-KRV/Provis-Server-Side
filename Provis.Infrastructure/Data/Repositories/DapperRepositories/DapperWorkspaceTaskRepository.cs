using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperWorkspaceTaskRepository : IWorkspaceTaskRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperWorkspaceTaskRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "Tasks";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(WorkspaceTask entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name, 
                                         Description,
                                         DateOfCreate,
                                         DateOfEnd,
                                         StoryPoints,
                                         StatusId,
                                         WorkspaceId,
                                         TaskCreatorId)
                                   VALUES 
                                        (@Name, 
                                         @Description,
                                         @DateOfCreate,
                                         @DateOfEnd,
                                         @StoryPoints,
                                         @StatusId,
                                         @WorkspaceId,
                                         @TaskCreatorId)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<WorkspaceTask> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name, 
                                         Description,
                                         DateOfCreate,
                                         DateOfEnd,
                                         StoryPoints,
                                         StatusId,
                                         WorkspaceId,
                                         TaskCreatorId)
                                   VALUES 
                                        (@Name, 
                                         @Description,
                                         @DateOfCreate,
                                         @DateOfEnd,
                                         @StoryPoints,
                                         @StatusId,
                                         @WorkspaceId,
                                         @TaskCreatorId)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(WorkspaceTask entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<WorkspaceTask> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<WorkspaceTask>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                                     ,[Description]
                                     ,[DateOfCreate]
                                     ,[DateOfEnd]
                                     ,[StatusId]
                                     ,[WorkspaceId]
                                     ,[TaskCreatorId]
                                     ,[StoryPoints]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<WorkspaceTask>(query);
            }
        }

        public async Task<WorkspaceTask> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                                     ,[Description]
                                     ,[DateOfCreate]
                                     ,[DateOfEnd]
                                     ,[StatusId]
                                     ,[WorkspaceId]
                                     ,[TaskCreatorId]
                                     ,[StoryPoints]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<WorkspaceTask>(query, new { Id = key[0] });
            }
        }

        public async Task<IEnumerable<WorkspaceTask>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<WorkspaceTask>(sql);
            }
        }

        public async Task UpdateAsync(WorkspaceTask entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET Name = @Name,
                                 Description = @Description,
                                 DateOfCreate = @DateOfCreate,
                                 DateOfEnd = @DateOfEnd,
                                 StoryPoints = @StoryPoints,
                                 StatusId = @StatusId,
                                 WorkspaceId = @WorkspaceId,
                                 TaskCreatorId = @TaskCreatorId";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<WorkspaceTask> IRepository<WorkspaceTask>.AddAsync(WorkspaceTask entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
