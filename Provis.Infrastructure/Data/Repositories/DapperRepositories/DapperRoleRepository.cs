using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.RoleEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperRoleRepository : IRoleRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperRoleRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "WorkspaceRoles";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(Role entity)
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

        public async Task AddRangeAsync(List<Role> entities)
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

        public async Task DeleteAsync(Role entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Role> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<Role>(query);
            }
        }

        public async Task<Role> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<Role>(query, new { Id = key[0]});
            }
        }

        public async Task<IEnumerable<Role>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<Role>(sql);
            }
        }

        public async Task UpdateAsync(Role entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                               SET Name = @Name";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<Role> IRepository<Role>.AddAsync(Role entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
