using Provis.Core.Entities.UserEntity;

namespace Provis.UnitTests.Base
{
    public class TestData
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
    }
}
