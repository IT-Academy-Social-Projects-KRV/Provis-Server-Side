using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces.Services;
using System;
using Provis.Core.Entities;
using Task = System.Threading.Tasks.Task;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Exeptions;
using Provis.Core.Roles;
using AutoMapper;
using Provis.Core.DTO.inviteUserDTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Provis.Core.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<Workspace> _workspace;
        protected readonly IRepository<UserWorkspace> _userWorkspace;
        protected readonly IRepository<InviteUser> _inviteUser;
        protected readonly IMapper _mapper;
        public WorkspaceService(UserManager<User> userManager,
            IRepository<Workspace> workspace,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<InviteUser> inviteUser,
            IMapper mapper)
        {
            _userManager = userManager;
            _workspace = workspace;
            _userWorkspace = userWorkspace;
            _inviteUser = inviteUser;
            _mapper = mapper;
        }
        public async Task CreateWorkspace(WorkspaceCreateDTO workspaceDTO, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            var workspace = _mapper.Map<Workspace>(workspaceDTO);
            workspace.DateOfCreate = DateTime.UtcNow;

            await _workspace.AddAsync(workspace);
            await _workspace.SaveChangesAsync();

            UserWorkspace userWorkspace = new UserWorkspace()
            {
                UserId = user.Id,
                WorkspaceId = workspace.Id,
                RoleId = WorkSpaceRoles.OwnerId
            };
            await _userWorkspace.AddAsync(userWorkspace);
            await _userWorkspace.SaveChangesAsync();

            await Task.CompletedTask;
        }


        public async Task DenyInviteAsync(InviteUserDTO inviteUserDTO, string userid)
        {
            var inviteUserRec = await _inviteUser.GetByKeyAsync(inviteUserDTO.Id);
            if (inviteUserRec == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Invite with with Id not found");
            }
            inviteUserRec.IsConfirm = false;
            await _inviteUser.SaveChangesAsync();

            await Task.CompletedTask;        
        }

        public async Task<List<WorkspaceInfoDTO>> GetWorkspaceListAsync(string userid)

        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            var listWorkspace = await _userWorkspace.Query().Where(y => y.UserId == userid).Include(x => x.Workspace).Include(x => x.Role).ToListAsync();

            var listWorkspaceToReturn = _mapper.Map<List<WorkspaceInfoDTO>>(listWorkspace);

            return listWorkspaceToReturn;
        }
    }
}
