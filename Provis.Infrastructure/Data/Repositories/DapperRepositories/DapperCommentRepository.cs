using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.Repositories.DapperRepositories
{
    class DapperCommentRepository : ICommentRepository
    {
        protected readonly IConfiguration _config;
        protected readonly string _tableName;

        public DapperCommentRepository(IConfiguration config)
        {
            _config = config;
            _tableName = "Comments";
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task AddAsync(Comment entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (CommentText, 
                                         DateOfCreate,
                                         TaskId,
                                         UserId)
                                   VALUES 
                                        (@CommentText, 
                                         @DateOfCreate,
                                         @TaskId,
                                         @UserId)";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task AddRangeAsync(List<Comment> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"INSERT INTO {_tableName} 
                                        (CommentText, 
                                         DateOfCreate,
                                         TaskId,
                                         UserId)
                                   VALUES 
                                        (@CommentText, 
                                         @DateOfCreate,
                                         @TaskId,
                                         @UserId)";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Comment entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Comment> entities)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"DELETE FROM [dbo].[{_tableName}]
                               WHERE Id = @Id";

                await db.ExecuteAsync(query, entities, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[CommentText]
                                     ,[DateOfCreate]
                                     ,[TaskId]
                                     ,[UserId]
                            FROM [dbo].[{_tableName}]";

                return await db.QueryAsync<Comment>(query);
            }
        }

        public async Task<Comment> GetByKeyAsync<TKey>(params TKey[] key)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"SELECT [Id]
                                     ,[CommentText]
                                     ,[DateOfCreate]
                                     ,[TaskId]
                                     ,[UserId]
                            FROM [dbo].[{_tableName}]
                            WHERE Id = @Id";

                return await db.QueryFirstAsync<Comment>(query, new { Id = key[0]});
            }
        }

        public async Task<IEnumerable<Comment>> Query(string sql)
        {
            using (IDbConnection db = Connection)
            {
                return await db.QueryAsync<Comment>(sql);
            }
        }

        public async Task UpdateAsync(Comment entity)
        {
            using (IDbConnection db = Connection)
            {
                var query = @$"UPDATE [dbo].[{_tableName}]
                             SET CommentText = @CommentText,
                                 DateOfCreate = @DateOfCreate,
                                 TaskId = @TaskId,
                                 UserId = @UserId";

                await db.ExecuteAsync(query, entity);
            }
        }

        Task<Comment> IRepository<Comment>.AddAsync(Comment entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
