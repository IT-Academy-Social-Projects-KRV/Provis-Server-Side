using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces.Services;
using System;
using Provis.Core.Entities;
using Task = System.Threading.Tasks.Task;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Exeptions;
using Provis.Core.Roles;
using Microsoft.EntityFrameworkCore;

namespace Provis.Core.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        protected readonly IEmailSenderService _emailSendService;
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<Workspace> _workspaceRepository;
        protected readonly IRepository<UserWorkspace> _userWorkspaceRepository;
        protected readonly IRepository<InviteUser> _inviteUserRepository;
        public WorkspaceService(UserManager<User> userManager, 
            IRepository<Workspace> workspace, 
            IRepository<UserWorkspace> userWorkspace,
            IRepository<InviteUser> inviteUser,
            IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _workspaceRepository = workspace;
            _userWorkspaceRepository = userWorkspace;
            _emailSendService = emailSenderService;
            _inviteUserRepository = inviteUser;
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
            await _workspaceRepository.AddAsync(workspace);
            await _workspaceRepository.SaveChangesAsync();

            UserWorkspace userWorkspace = new UserWorkspace()
            {
                UserId = user.Id,
                WorkspaceId = workspace.Id,
                RoleId = WorkSpaceRoles.OwnerId
            };
            await _userWorkspaceRepository.AddAsync(userWorkspace);
            await _userWorkspaceRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task SendInviteAsync(InviteUserDTO inviteUser, string userId)
        {
            var invitingUser = await _userManager.FindByEmailAsync(inviteUser.UserEmail);
            var owner = await _userManager.FindByIdAsync(userId);

            if (invitingUser == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Email not exist");
            }

            var workspace = await _workspaceRepository.GetByKeyAsync(inviteUser.WorkspaceId);

            if (workspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Not found this workspace");
            }

            var inviteUserColumn = await _inviteUserRepository.Query().FirstOrDefaultAsync(x =>
            x.FromUserId == userId &&
            x.ToUserId == invitingUser.Id &&
            x.WorkspaceId == workspace.Id);

            if (inviteUserColumn != null)
            {
                throw new HttpException(System.Net.HttpStatusCode.Conflict, "This user already have invite, wait for a answer");
            }

            InviteUser user = new InviteUser
            {
                Date = DateTime.UtcNow,
                FromUser = owner,
                ToUser = invitingUser,
                Workspace = workspace
            };

            await _inviteUserRepository.AddAsync(user);
            await _inviteUserRepository.SaveChangesAsync();

            await _emailSendService.SendAsync(invitingUser.Email, $"Owner: {owner.UserName} - Welcome to my Workspace");

            await Task.CompletedTask;
        }
    }
}
