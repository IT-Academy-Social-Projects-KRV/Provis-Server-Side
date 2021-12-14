using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces.Services;
using System;
using Provis.Core.Entities;
using Task = System.Threading.Tasks.Task;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Exeptions;
using Provis.Core.Roles;

namespace Provis.Core.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<Workspace> _workspace;
        protected readonly IRepository<UserWorkspace> _userWorkspace;
        public WorkspaceService(UserManager<User> userManager, IRepository<Workspace> workspace, IRepository<UserWorkspace> userWorkspace)
        {
            _userManager = userManager;
            _workspace = workspace;
            _userWorkspace = userWorkspace;
        }
        public async Task CreateWorkspace(WorkspaceCreateDTO workspaceDTO, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            Workspace workspace = new Workspace()
            {
                DateOfCreate = DateTime.UtcNow,
                Name = workspaceDTO.Name,
                Description = workspaceDTO.Description
            };
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
    }
}
