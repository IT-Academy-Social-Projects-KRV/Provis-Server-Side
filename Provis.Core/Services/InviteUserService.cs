using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Services
{
    public class InviteUserService : IInviteUserService
    {
        protected readonly IEmailSenderService _emailSendService;
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<InviteUser> _inviteUserRepository;
        protected readonly IRepository<Workspace> _workspaceRepositoryRepository;

        public InviteUserService(IEmailSenderService emailSenderService, 
            UserManager<User> userManager, 
            IRepository<InviteUser> inviteUser,
            IRepository<Workspace> workspaceRepository)
        {
            _emailSendService = emailSenderService;
            _userManager = userManager;
            _inviteUserRepository = inviteUser;
            _workspaceRepositoryRepository = workspaceRepository;
        }

        public async Task SendInviteAsync(InviteUserDTO inviteUser, string userId)
        {
            var invitingUser = await _userManager.FindByEmailAsync(inviteUser.UserEmail);
            var owner = await _userManager.FindByIdAsync(userId);

            if(invitingUser == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Email not exist");
            }

            var workspace = await _workspaceRepositoryRepository.GetByKeyAsync(inviteUser.WorkspaceId);

            if(workspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Not found this workspace");
            }

            var inviteUserColumn = await _inviteUserRepository.Query().FirstOrDefaultAsync(x => 
            x.FromUserId == userId &&
            x.ToUserId == invitingUser.Id &&
            x.WorkspaceId == workspace.Id);

            if(inviteUserColumn != null)
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
