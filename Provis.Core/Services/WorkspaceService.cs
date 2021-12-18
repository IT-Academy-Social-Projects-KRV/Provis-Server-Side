using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using System;
using Task = System.Threading.Tasks.Task;

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

        public async Task SendInviteAsync(InviteUserDTO inviteDTO, string ownerId)
        {
            var inviteUser = await _userManager.FindByEmailAsync(inviteDTO.UserEmail);
            var owner = await _userManager.FindByIdAsync(ownerId);

            if (inviteUser == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with this Email not exist");
            }

            var workspace = await _workspaceRepository.GetByKeyAsync(inviteDTO.WorkspaceId);

            if (workspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Not found this workspace");
            }

            // Check privacy, temporary solution

            var checkRole = await _userWorkspaceRepository.Query().FirstOrDefaultAsync(u => u.WorkspaceId == inviteDTO.WorkspaceId && u.UserId == ownerId);

            if(checkRole == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.UnavailableForLegalReasons, "You don't have permissions!");
            }
            else
            {

                if (checkRole.RoleId == WorkSpaceRoles.MemberId || checkRole.RoleId == WorkSpaceRoles.ViewerId)
                {
                    throw new HttpException(System.Net.HttpStatusCode.UnavailableForLegalReasons, "You don't have permissions!");
                }

                var inviteUserColumn = await _inviteUserRepository.Query().FirstOrDefaultAsync(x =>
                    x.FromUserId == ownerId &&
                    x.ToUserId == inviteUser.Id &&
                    x.WorkspaceId == workspace.Id);

                if (inviteUserColumn != null)
                {
                    throw new HttpException(System.Net.HttpStatusCode.UnavailableForLegalReasons, "This user already have invite, wait for a answer");
                }

                InviteUser user = new InviteUser
                {
                    Date = DateTime.UtcNow,
                    FromUser = owner,
                    ToUser = inviteUser,
                    Workspace = workspace
                };

                await _inviteUserRepository.AddAsync(user);
                await _inviteUserRepository.SaveChangesAsync();

                await _emailSendService.SendAsync(inviteUser.Email, $"Owner: {owner.UserName} - Welcome to my Workspace {workspace.Name}");

                await Task.CompletedTask;
            }
        }
    }
}
