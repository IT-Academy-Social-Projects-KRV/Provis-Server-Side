using Provis.Core.Entities.UserEntity;
using Provis.Infrastructure.Data;
using System.Linq;

namespace Provis.IntegrationTest.Data
{
    public class UsersData
    {
        public static string currentUserId = "1";
        public static void InsertCurrentUser(ProvisDbContext dbContext)
        {
            dbContext.Users.Add(new User()
            {
                Id = currentUserId,
                Email = "test1@gmail.com",
                Name = "Name1",
                Surname = "Surname1",
                UserName = "Username1",
                ImageAvatarUrl = "Path1"
            });

            dbContext.SaveChanges();
        }

        public static User GetCurrentUser(ProvisDbContext dbContext)
        {
            return dbContext.Users.FirstOrDefault(x=>x.Id == currentUserId);
        }

    }
}
