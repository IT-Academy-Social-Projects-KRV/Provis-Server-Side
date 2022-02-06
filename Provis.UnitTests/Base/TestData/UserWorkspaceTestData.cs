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
                }
            };
        }
    }
}
