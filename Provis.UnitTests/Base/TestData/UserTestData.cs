using Provis.Core.Entities.UserEntity;
using System.Collections.Generic;

namespace Provis.UnitTests.Base.TestData
{
    public class UserTestData
    {
        public static User GetTestUser()
        {
            return new User()
            {
                Id = "1",
                Email = "test1@gmail.com",
                Name = "Name1",
                Surname = "Surname1",
                UserName = "Username1",
                ImageAvatarUrl = "Path1"
            };
        }

        public static List<User> GetTestUserList()
        {
            return new List<User>()
            {
                new User()
                {
                    Id = "1",
                    Email = "test1@gmail.com",
                    Name = "Name1",
                    Surname = "Surname1",
                    UserName = "Username1",
                    ImageAvatarUrl = "Path1"
                },

                new User()
                {
                    Id = "2",
                    Email = "test2@gmail.com",
                    Name = "Name2",
                    Surname = "Surname2",
                    UserName = "Username2",
                    ImageAvatarUrl = "Path2"
                }
            };
        }
    }
}
