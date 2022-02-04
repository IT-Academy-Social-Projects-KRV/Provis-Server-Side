using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.WorkspaceTaskAttachmentEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperWorkspaceTaskAttachmentRepository : IWorkspaceTaskAttachmentRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperWorkspaceTaskAttachmentRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "TaskAttachments";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(WorkspaceTaskAttachment entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (AttachmentPath, 
                                         TaskId)
                                   VALUES 
                                        (@AttachmentPath, 
                                         @TaskId)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<WorkspaceTaskAttachment> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (AttachmentPath, 
                                         TaskId)
                                   VALUES 
                                        (@AttachmentPath, 
                                         @TaskId)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(WorkspaceTaskAttachment entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<WorkspaceTaskAttachment> entities)
        {
            using(IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<WorkspaceTaskAttachment>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[AttachmentPath]
                                     ,[TaskId]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<WorkspaceTaskAttachment>(query);
            }
        }

        public async Task<WorkspaceTaskAttachment> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[AttachmentPath]
                                     ,[TaskId]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<WorkspaceTaskAttachment>(query, new { Id = key[0] });
            }
        }

        public async Task<IEnumerable<WorkspaceTaskAttachment>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<WorkspaceTaskAttachment>(sql);
            }
        }

        public async Task UpdateAsync(WorkspaceTaskAttachment entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET AttachmentPath = @AttachmentPath,
                                 TaskId = @TaskId";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<WorkspaceTaskAttachment> IRepository<WorkspaceTaskAttachment>.AddAsync(WorkspaceTaskAttachment entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
