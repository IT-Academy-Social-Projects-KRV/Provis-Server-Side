using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperUserRoleTagRepository : IUserRoleTagRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperUserRoleTagRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "UserRoleTags";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(UserRoleTag entity)
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

        public async Task AddRangeAsync(List<UserRoleTag> entities)
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

        public async Task DeleteAsync(UserRoleTag entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<UserRoleTag> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<UserRoleTag>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<UserRoleTag>(query);
            }
        }

        public async Task<UserRoleTag> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<UserRoleTag>(query, new { Id = key[0] });
            }
        }

        public async Task<IEnumerable<UserRoleTag>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<UserRoleTag>(sql);
            }
        }

        public async Task UpdateAsync(UserRoleTag entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET Name = @Name";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<UserRoleTag> IRepository<UserRoleTag>.AddAsync(UserRoleTag entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
