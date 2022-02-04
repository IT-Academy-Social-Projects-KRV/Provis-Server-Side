using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperWorkspaceRepository : IWorkspaceRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperWorkspaceRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "Workspaces";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<Workspace> AddAsync(Workspace entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name, 
                                         Description,
                                         DateOfCreate)
                                   VALUES 
                                        (@Name, 
                                         @Description,
                                         @DateOfCreate)
                            SELECT CAST(SCOPE_IDENTITY() as INT)";

                var res = await db.QueryAsync<int>(query, entity);
                int? id = res.AsList().FirstOrDefault();

                entity.Id = id.Value;
                return entity;
            }
        }

        public async Task AddRangeAsync(List<Workspace> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name, 
                                         Description,
                                         DateOfCreate)
                                   VALUES 
                                        (@Name, 
                                         @Description,
                                         @DateOfCreate)
                            SELECT CAST(SCOPE_IDENTITY() as INT)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Workspace entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Workspace> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Workspace>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                                     ,[Description]
                                     ,[DateOfCreate]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<Workspace>(query);
            }
        }

        public async Task<Workspace> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                                     ,[Description]
                                     ,[DateOfCreate]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<Workspace>(query, new { Id = key[0] });
            }
        }

        public async Task<IEnumerable<Workspace>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<Workspace>(sql);
            }
        }

        public async Task UpdateAsync(Workspace entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET Name = @Name,
                                 Description = @Description,
                                 DateOfCreate = @DateOfCreate";

                await db.ExecuteAsync(query, entity);
            }
        }
    }
}
