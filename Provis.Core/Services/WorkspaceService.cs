using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces.Services;
using System;
using Provis.Core.Entities;
using Task = System.Threading.Tasks.Task;
using Provis.Core.Interfaces.Repositories;

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
        public async Task CreateWorkspase(string userid)
        {
            //all roles must already be in the database!!
            var user = await _userManager.FindByIdAsync(userid);

            Workspace workspace = new Workspace()
            {
                DateOfCreate = DateTime.UtcNow,
                Name = "New Workspace",
                Description = "Description"
            };
            await _workspace.AddAsync(workspace);
            await _workspace.SaveChangesAsync();

            UserWorkspace userWorkspace = new UserWorkspace()
            {
                UserId = userid,
                WorkspaceId = workspace.Id,
                RoleId = 1
            };
            await _userWorkspace.AddAsync(userWorkspace);
            await _userWorkspace.SaveChangesAsync();

            await Task.CompletedTask;
        }
    }
}
