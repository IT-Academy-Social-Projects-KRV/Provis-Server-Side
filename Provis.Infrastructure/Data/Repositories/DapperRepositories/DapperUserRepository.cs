using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperUserRepository : IUserRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperUserRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "AspNetUsers";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(User entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name,
                                         Surname, 
                                         ImageAvatarUrl,
                                         CreateDate)
                                   VALUES 
                                        (@Name, 
                                         @Surname, 
                                         @ImageAvatarUrl,
                                         @CreateDate)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<User> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (Name,
                                         Surname, 
                                         ImageAvatarUrl,
                                         CreateDate)
                                   VALUES 
                                        (@Name, 
                                         @Surname, 
                                         @ImageAvatarUrl,
                                         @CreateDate)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task DeleteAsync(User entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<User> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                                     ,[Surname]
                                     ,[ImageAvatarUrl]
                                     ,[CreateDate]
                                     ,[UserName]
                                     ,[NormalizedUserName]
                                     ,[Email]
                                     ,[NormalizedEmail]
                                     ,[EmailConfirmed]
                                     ,[ConcurrencyStamp]
                                     ,[TwoFactorEnabled]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<User>(query);
            }
        }

        public async Task<User> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[Name]
                                     ,[Surname]
                                     ,[ImageAvatarUrl]
                                     ,[CreateDate]
                                     ,[UserName]
                                     ,[NormalizedUserName]
                                     ,[Email]
                                     ,[NormalizedEmail]
                                     ,[EmailConfirmed]
                                     ,[ConcurrencyStamp]
                                     ,[TwoFactorEnabled]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<User>(query, new { Id = key[0] });
            }
        }

        public async Task<IEnumerable<User>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<User>(sql);
            }
        }

        public async Task UpdateAsync(User entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET Name = @Name, 
                                 Surname = @Surname,
                                 ImageAvatarUrl = @ImageAvatarUrl,
                                 CreateDate = @CreateDate";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<User> IRepository<User>.AddAsync(User entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
