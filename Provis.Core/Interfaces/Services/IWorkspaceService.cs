﻿using Provis.Core.DTO.workspaceDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IWorkspaceService 
    {
        Task CreateWorkspace(WorkspaceCreateDTO workspaceDTO, string userid);
      
        Task DenyInviteAsync(int id, string userid);
      
        Task<List<WorkspaceInfoDTO>> GetWorkspaceListAsync(string userid);

        Task SendInviteAsync(InviteUserDTO inviteUser, string userId);
    }
}
