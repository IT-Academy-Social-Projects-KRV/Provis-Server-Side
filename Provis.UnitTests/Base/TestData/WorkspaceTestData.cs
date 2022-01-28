using Provis.Core.DTO.WorkspaceDTO;
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

        public static Workspace CreateWorkspace(WorkspaceCreateDTO workspaceDTO, string userid)
        {
            return new Workspace()
            {
                Id = 1,
                DateOfCreate = DateTime.Today,
                Description = "Description1",
                Name = "Name1"
            };
        }
    }
}
