using Provis.Infrastructure.Data;

namespace Provis.IntegrationTest.Data
{
    public class DatabaseInitialization
    {
        public static void InitializeDatabase(ProvisDbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            UsersData.InsertCurrentUser(dbContext);
        }
    }
}
