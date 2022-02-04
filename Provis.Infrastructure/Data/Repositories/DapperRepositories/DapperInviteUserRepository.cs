using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperInviteUserRepository : IInviteUserRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperInviteUserRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "InviteUsers";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(InviteUser entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Date, 
                                         IsConfirm,
                                         Workspace,
                                         FromUserId,
                                         ToUserId)
                                   VALUES 
                                        (@Date, 
                                         @IsConfirm,
                                         @Workspace,
                                         @FromUserId,
                                         @ToUserId)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<InviteUser> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Date, 
                                         IsConfirm,
                                         Workspace,
                                         FromUserId,
                                         ToUserId)
                                   VALUES 
                                        (@Date, 
                                         @IsConfirm,
                                         @Workspace,
                                         @FromUserId,
                                         @ToUserId)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(InviteUser entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<InviteUser> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<InviteUser>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Date]
                                     ,[IsConfirm]
                                     ,[WorkspaceId]
                                     ,[FromUserId]
                                     ,[ToUserId]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<InviteUser>(query);
            }
        }

        public async Task<InviteUser> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Date]
                                     ,[IsConfirm]
                                     ,[WorkspaceId]
                                     ,[FromUserId]
                                     ,[ToUserId]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<InviteUser>(query, new { Id = key[0]});
            }
        }

        public async Task<IEnumerable<InviteUser>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<InviteUser>(sql);
            }
        }

        public async Task UpdateAsync(InviteUser entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET Date = @Date, 
                                 IsConfirm = @IsConfirm,
                                 Workspace = @Workspace,
                                 FromUserId = @FromUserId,
                                 ToUserId = @ToUserId";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<InviteUser> IRepository<InviteUser>.AddAsync(InviteUser entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
