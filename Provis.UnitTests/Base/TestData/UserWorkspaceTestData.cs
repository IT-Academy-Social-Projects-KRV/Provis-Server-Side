using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using System.Collections.Generic;

namespace Provis.UnitTests.Base.TestData
{
    public class UserWorkspaceTestData
    {
        public static List<UserWorkspace> GetTestUserWorkspaceList()
        {
            return new List<UserWorkspace>()
            { 
                new UserWorkspace
                {
                    UserId = "1",
                    RoleId = 1,
                    WorkspaceId = 1
                },

                new UserWorkspace
                {
                    UserId = "2",
                    RoleId = 2,
                    WorkspaceId = 1
                },

                new UserWorkspace
                {
                    UserId = "3",
                    RoleId = 1,
                    WorkspaceId = 1,
                    User = new User
                    {
                        Id = "3",
                        Email = "test3@gmail.com",
                        Name = "Name3",
                        Surname = "Surname3",
                        UserName = "Username3",
                        ImageAvatarUrl = "Path3"
                    }
                },

                new UserWorkspace
                {
                    UserId = "4",
                    RoleId = 3,
                    WorkspaceId = 1,
                    User = new User
                    {
                        Id = "4",
                        Email = "test4@gmail.com",
                        Name = "Name4",
                        Surname = "Surname4",
                        UserName = "Username4",
                        ImageAvatarUrl = "Path4"
                    }
                },
            };
        }
    }
}
