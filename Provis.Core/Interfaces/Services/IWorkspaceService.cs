using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IWorkspaceService 
    {
        Task CreateWorkspace(WorkspaceCreateDTO workspaceDTO, string userid);
        Task AcceptInvitation(string userId);
    }
}
