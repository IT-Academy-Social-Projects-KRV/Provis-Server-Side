using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.RefreshTokenEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperRefreshTokenRepository : IRefreshTokenRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperRefreshTokenRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "RefreshTokens";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(RefreshToken entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Token, 
                                         UserId)
                                   VALUES 
                                        (@Token, 
                                         @UserId)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<RefreshToken> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Token, 
                                         UserId)
                                   VALUES 
                                        (@Token, 
                                         @UserId)";

                await db.ExecuteAsync(query, entities,  commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(RefreshToken entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<RefreshToken> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Token]
                                     ,[UserId]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<RefreshToken>(query);
            }
        }

        public async Task<RefreshToken> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Token]
                                     ,[UserId]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<RefreshToken>(query, new { Id = key[0]});
            }
        }

        public async Task<IEnumerable<RefreshToken>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<RefreshToken>(sql);
            }
        }

        public async Task UpdateAsync(RefreshToken entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                               SET Token = @Token, 
                                   UserId = @UserId";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<RefreshToken> IRepository<RefreshToken>.AddAsync(RefreshToken entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
