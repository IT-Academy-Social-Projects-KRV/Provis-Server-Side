using Provis.Core.DTO.WorkspaceDTO;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.UnitTests.Base.TestData
{
    public class WorkspaceTestData
    {
        public static WorkspaceCreateDTO GetWorkspaceCreateDTO()
        {
            return new WorkspaceCreateDTO()
            {
                Name = "Name1",
                Description = "Description1"
            };
        }

        public static Workspace GetTestWorkspace()
        {
            return new Workspace()
            {
                Id = 1,
                Description = "Description1",
                Name = "Name1",
                DateOfCreate = DateTime.Now
            };
        }

        public static UserWorkspace GetTestUserWorkspace()
        {
            return new UserWorkspace()
            {
                UserId = "1",
                WorkspaceId = 1,
                RoleId = 1
            };
        }
    }
}
